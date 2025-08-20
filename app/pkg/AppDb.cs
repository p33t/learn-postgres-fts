using app.pkg.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace app.pkg;

public class AppDb(DbContextOptions baseSetup) : DbContext(baseSetup)
{
    public DbSet<LibRes> LibRes { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // require explicit .AsTracking()
            // For debugging... .AddCommandLogging()
            .ReplaceService<ISqlGenerationHelper, SqlGenerationHelper>();
    }
    
}