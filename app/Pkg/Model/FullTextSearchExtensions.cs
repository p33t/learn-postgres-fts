using Microsoft.EntityFrameworkCore;

namespace app.Pkg.Model;

public static class FullTextSearchExtensions
{
    /// Performs the appropriate where statement for an indirect full-text-search using the given language
    public static IQueryable<TEntity> FullTextSearch<TEntity>(this IQueryable<TEntity> query, string term,
        string language) where TEntity : IHasId, IHasFtses
    {
        if ("english".Equals(language, StringComparison.InvariantCultureIgnoreCase))
        {
            return query.Where(hr =>
                hr.Ftses!.Any(fts =>
                    fts.VectorEn.Matches(EF.Functions.PlainToTsQuery("english", term))));
        }

        if ("french".Equals(language, StringComparison.InvariantCultureIgnoreCase))
        {
            return query.Where(hr =>
                hr.Ftses!.Any(fts =>
                    fts.VectorFr.Matches(EF.Functions.PlainToTsQuery("french", term))));
        }

        throw new NotSupportedException($"Language '{language}' is not supported.");
    }

    public static void AddFullTextSearch<TEntity>(this ModelBuilder modelBuilder) where TEntity : class, IHasFtses
    {
        /* TODO: Convert to 'owned' because model cannot be shared by top-level entities. But will include by default.
         
        modelBuilder.Entity<TEntity>().OwnsMany(s => s.Ftses,
           typeBuilder =>
           {
               typeBuilder.ToTable($"{typeof(TEntity).Name}Fts")
                   .WithOwner()
                   .HasForeignKey(l => l.OwnerId);

               typeBuilder
                   .HasIndex(x => x.OwnerId) // Will add 'Language' if more translations are needed
                   .IsUnique();
               
               typeBuilder.Property(x => x.En)
                   .IsGeneratedTsVectorColumn(SearchLangEnum.En.PostgresLanguage(), nameof(Fts.TextA));
               typeBuilder.HasIndex(x => x.En).HasMethod("GIN");
               
               typeBuilder.Property(x => x.Fr)
                   .IsGeneratedTsVectorColumn(SearchLangEnum.Fr.PostgresLanguage(), nameof(Fts.TextA));
               typeBuilder.HasIndex(x => x.Fr).HasMethod("GIN");
           });         
         */
        
        
        var typeBuilder = modelBuilder.Entity<Fts>()
            .ToTable($"{typeof(TEntity).Name}Fts");

        typeBuilder
            .HasOne<TEntity>()
            .WithMany(x => x.Ftses)
            .HasForeignKey(nameof(Fts.OwnerId))
            .OnDelete(DeleteBehavior.Cascade);

        typeBuilder
            .HasIndex(x => x.OwnerId) // Will add 'Language' if more translations are needed
            .IsUnique();

        typeBuilder
            .HasGeneratedTsVectorColumn(x => x.VectorEn,
                "english",
                x => new { x.TextA })
            .HasIndex(x => x.VectorEn)
            .HasMethod("GIN");

        typeBuilder
            .HasGeneratedTsVectorColumn(x => x.VectorFr,
                "french",
                x => new { x.TextA })
            .HasIndex(x => x.VectorFr)
            .HasMethod("GIN");
    }
}