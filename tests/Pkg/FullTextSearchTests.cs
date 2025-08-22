using app.Pkg;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Infra;

namespace tests.Pkg;

public class FullTextSearchTests(TestFixture fixture): TestsBase
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
}