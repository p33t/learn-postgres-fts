using app.Pkg;
using app.Pkg.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tests.Infra;

namespace tests.Pkg;

public class BasicDbTests(TestFixture fixture) : TestsBase
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
}