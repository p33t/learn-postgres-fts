using app;
using app.Pkg;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace tests.Infra;

public class TestFixture
{
    private readonly IHost _host = Program.CreateHost([]);
    public IServiceProvider Services => _host.Services;

    /// Executes the given function within a DI 'scope'
    public async Task<T> WithScopeAsync<T>(Func<IServiceProvider, Task<T>> fn)
    {
        using var scope = Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        return await fn(scope.ServiceProvider);
    }

    /// Variant of <see cref="WithScopeAsync{T}"/> without type args
    public Task WithScopeAsync(Func<IServiceProvider, Task> fn) =>
        WithScopeAsync<string>(async sp =>
        {
            await fn(sp);
            return string.Empty;
        });

    /// Clears the database
    public async Task ResetAsync()
    {
        using var scope = Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDb>();
        await db.LibRes.ExecuteDeleteAsync();
    }
}