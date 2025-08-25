using app.Pkg;
using app.Pkg.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Infra;
using Xunit.Abstractions;

namespace tests.Pkg;

public class IndexModificationTests(TestFixture fixture, ITestOutputHelper outputHelper) : TestsBase
{
    [Fact]
    public async Task UpdatingV2()
    {
        // fixture.SetupLogging(outputHelper);

        var id = Guid.NewGuid().ToString();
        var altText = $"Alternative text for {id}";
        
        await fixture.WithScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<AppDb>();

            var hr = new HotelReview2
            {
                Id = id,
                Title = $"Title for {id}",
                Text = $"Text for {id}",
                Property = "test property",
                Rating = 5
            };
            hr.UpdateFtses();
            
            db.HotelReview2.Add(hr);
            await db.SaveChangesAsync();
        });

        await fixture.WithScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<AppDb>();
            
            var hr = await db.HotelReview2.AsTracking().Include(x => x.Ftses)
                .SingleAsync(x => x.Id == id);

            hr.Text = altText;
            hr.UpdateFtses();
            var count = await db.SaveChangesAsync();
            Assert.Equal(2, count);
        });
        
        await fixture.WithScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<AppDb>();
            
            var hr = await db.HotelReview2.AsTracking().Include(x => x.Ftses)
                .FullTextSearch($"alternative {id}", "english")
                .FirstOrDefaultAsync(x => x.Id == id);

            Assert.NotNull(hr);
            Assert.Equal(altText, hr.Text); // changes were saved

            db.HotelReview2.Remove(hr);
            await db.SaveChangesAsync();
            
            // Confirm record gone
            Assert.Null(db.HotelReview2.FirstOrDefault(x => x.Id == id));
        });
    }

    [Fact]
    public async Task UpdateMappedEntry_DoesNotWork()
    {
        // fixture.SetupLogging(outputHelper);

        await fixture.WithScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<AppDb>();

            var review0 = await db.HotelReview.AsTracking()
                .OrderBy(x => x.Id)
                .Select(HotelReview.LEAN)
                .FirstAsync();

            review0.Text = $"random value {Random.Shared.Next()}";
            var count = await db.SaveChangesAsync();
            Assert.Equal(0, count); // >>>> isn't saved
        });
    }

    [Fact]
    public async Task UpdateMappedEntry_WorksIfManuallyAttached()
    {
        // fixture.SetupLogging(outputHelper);

        await fixture.WithScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<AppDb>();

            var review0 = await db.HotelReview
                .OrderBy(x => x.Id)
                .Select(HotelReview.LEAN)
                .FirstAsync();

            db.HotelReview.Attach(review0);
            var origText = review0.Text;
            var uniqueString = "thisuniquestringshouldonlyoccuronce";
            review0.Text = uniqueString;
            var count = await db.SaveChangesAsync();
            Assert.Equal(1, count);
            db.Entry(review0).State = EntityState.Detached;

            var altReview0 = await db.HotelReview.AsTracking()
                .Where(x => x.VectorEn.Matches(EF.Functions.PlainToTsQuery("english", uniqueString)))
                .Select(HotelReview.LEAN)
                .SingleAsync();

            db.HotelReview.Attach(altReview0);
            altReview0.Text = origText;
            var altCount = await db.SaveChangesAsync();
            Assert.Equal(1, altCount);
        });
    }
}