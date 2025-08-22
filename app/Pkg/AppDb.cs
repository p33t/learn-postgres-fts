using app.Pkg.Model;
using app.Pkg.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace app.Pkg;

public class AppDb(DbContextOptions baseSetup, IDbLogTo logTo) : DbContext(baseSetup)
{
    public DbSet<LibRes> LibRes { get; set; }

    public DbSet<HotelReview> HotelReview { get; set; }
    
    public DbSet<HotelReview2> HotelReview2 { get; set; }
    
    public DbSet<HotelReview2Fts> HotelReview2Fts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // require explicit .AsTracking()
            // For debugging...
            .LogTo(logTo.WriteLine)
            .ReplaceService<ISqlGenerationHelper, SqlGenerationHelper>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // V1
        modelBuilder.Entity<HotelReview>()
            .HasGeneratedTsVectorColumn(x => x.VectorEn,
                "english",
                x => new { x.Title, x.Text })
            .HasIndex(x => x.VectorEn)
            .HasMethod("GIN");

        // V2
        modelBuilder.Entity<HotelReview2>()
            .HasMany(x => x.Ftss)
            .WithOne(x => x.Owner)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<HotelReview2Fts>()
            .HasIndex(x => x.OwnerId) // Will add 'Language' if more translations are needed
            .IsUnique();
            
        modelBuilder.Entity<HotelReview2Fts>()
            .HasGeneratedTsVectorColumn(x => x.VectorEn,
                "english",
                x => new { x.TextA })
            .HasIndex(x => x.VectorEn)
            .HasMethod("GIN");
        
        modelBuilder.Entity<HotelReview2Fts>()
            .HasGeneratedTsVectorColumn(x => x.VectorFr,
                "french",
                x => new { x.TextA })
            .HasIndex(x => x.VectorFr)
            .HasMethod("GIN");
    }
}