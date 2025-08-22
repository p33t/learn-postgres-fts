using app.Pkg;
using app.Pkg.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Infra;
using Xunit.Abstractions;

namespace tests.Pkg;

public class FullTextSearchTests(TestFixture fixture, ITestOutputHelper outputHelper) : TestsBase
{
    [Theory]
    [InlineData("marry", 33, 26)]
    [InlineData("beauty", 1497, 1453)]
    public async Task SimpleTermSearch(string term, int expCount, int expNonTermCount)
    {
        await fixture.WithScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<AppDb>();
            var result = await db.HotelReview
                .Where(x => x.VectorEn.Matches(EF.Functions.PlainToTsQuery("english", term)))
                .ToListAsync();

            Assert.Equal(expCount, result.Count);

            // Confirm the term wasn't in some of the results
            var nonTermCount = result.Count(x => !x.Text.ToLowerInvariant().Contains(term)
                                                 && !x.Title.ToLowerInvariant().Contains(term));

            Assert.Equal(expNonTermCount, nonTermCount);
        });
    }

    [Theory]
    [InlineData("marry", "english", 33)]
    [InlineData("marry", "french", 7)]
    [InlineData("beautiful", "english", 1497)]
    [InlineData("beautiful", "french", 1233)]
    [InlineData("petit", "english", 216)]
    [InlineData("petit", "french", 187)] // hmm... you'd think this would have more
    public async Task LanguageSpecificSearch(string term, string language, int expCount)
    {
        fixture.SetupLogging(outputHelper);
        
        await fixture.WithScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<AppDb>();
            var result = await db.HotelReview2
                .FullTextSearch(term, language)
                .ToListAsync();

            Assert.Equal(expCount, result.Count);
        });
    }
}