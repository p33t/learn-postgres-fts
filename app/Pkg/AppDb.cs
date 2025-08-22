using app.Pkg.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace app.Pkg;

public class AppDb(DbContextOptions baseSetup) : DbContext(baseSetup)
{
    public DbSet<LibRes> LibRes { get; set; }

    public DbSet<HotelReview> HotelReview { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // require explicit .AsTracking()
            // For debugging... .AddCommandLogging()
            .ReplaceService<ISqlGenerationHelper, SqlGenerationHelper>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<HotelReview>()
            .HasGeneratedTsVectorColumn(x => x.VectorEn,
                "english",
                x => new { x.Title, x.Text })
            .HasIndex(x => x.VectorEn)
            .HasMethod("GIN");

        // Not sure how to tell EF not to load certain fields. Maybe this happens by default.
        // modelBuilder.Entity<HotelReview>()
        //     .Navigation(x => x.VectorEn)
        //     .AutoInclude(false);
    }
}