using app.pkg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
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

using IHost host = builder.Build();
await host.RunAsync();
