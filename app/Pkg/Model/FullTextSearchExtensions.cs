using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace app.Pkg.Model;

public static class FullTextSearchExtensions
{
    /// Performs the appropriate where statement for a direct full-text-search using the given language
    public static IQueryable<Fts> FullTextSearch(this IQueryable<Fts> query, string term,
        string language)
    {
        if ("english".Equals(language, StringComparison.InvariantCultureIgnoreCase))
        {
            return query.Where(x => x.VectorEn.Matches(EF.Functions.PlainToTsQuery("english", term)));
        }

        if ("french".Equals(language, StringComparison.InvariantCultureIgnoreCase))
        {
            return query.Where(x => x.VectorFr.Matches(EF.Functions.PlainToTsQuery("french", term)));
        }

        throw new NotSupportedException($"Language '{language}' is not supported.");
    }

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