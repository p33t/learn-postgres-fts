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
        fixture.SetupLogging(outputHelper);
        
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
}