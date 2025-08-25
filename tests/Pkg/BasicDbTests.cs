using app.Pkg;
using app.Pkg.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Infra;
using Xunit.Abstractions;

namespace tests.Pkg;

public class BasicDbTests(TestFixture fixture, ITestOutputHelper outputHelper) : TestsBase
{
    [Fact]
    public async Task InsertAndRetrieve()
    {
        var testName = GetCurrentMethod().CalcTestName();
        await fixture.ResetAsync<LibRes>();
        await fixture.WithScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<AppDb>();
            var entity = db.LibRes.Add(new()
            {
                Id = Guid.NewGuid().ToString(),
                Title = $"Title {testName}",
                Abstract = $"Abstract {testName}"
            });
            await db.SaveChangesAsync();
            entity.State = EntityState.Detached;
            var added = entity.Entity;

            var saved = await db.LibRes.SingleAsync(x => x.Id == added.Id);
            Assert.Equal(added.Title, saved.Title);
            Assert.Equivalent(added, saved);
        });
    }

    /// One way to avoid loading unnecessary fields
    [Fact]
    public async Task Select_AvoidsLoadingVectorFields()
    {
        // fixture.SetupLogging(outputHelper);

        await fixture.WithScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<AppDb>();

            var results = await db.HotelReview
                .Where(x => x.VectorEn.Matches(EF.Functions.PlainToTsQuery("conversation")))
                // NOTE: Must be after where clause
                // .Select(HotelReview.LEAN)
                // .ToListAsync();
                .ToListLeanAsync();

            Assert.NotEmpty(results);
            Assert.All(results, x => Assert.Null(x.VectorEn));
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