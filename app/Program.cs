using app.Pkg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace app;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHost(args);
        await host.RunAsync();
    }

    public static IHost CreateHost(string[] strings)
    {
        IHost? host1 = null;
        try
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(strings);
            var services = builder.Services;
            services.AddDbContext<AppDb>((_, dbOptions) =>
            {
                var connectionString = "Host=postgres;Username=learn-login;Password=learn-login;Database=learn-db";
                // Suppress a warning about multiple ServiceProvider instances (ManyServiceProvidersCreatedWarning)
                dbOptions.ConfigureWarnings(wb => wb.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))
                    .UseNpgsql(connectionString, postgresOptions =>
                        // Allows multiple DbContexts in the same schema
                        postgresOptions.MigrationsHistoryTable($"{HistoryRepository.DefaultTableName}_fts"
                            .ToLowerInvariant()));
            });

            host1 = builder.Build();
            return host1;
        }
        catch
        {
            host1?.Dispose();
            throw;
        }
    }
}