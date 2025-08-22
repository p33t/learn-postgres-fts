using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace app.Pkg.Model;

public interface IFtsEntity
{
    string OwnerId { get; set; }
    
    public NpgsqlTsVector VectorEn { get; set; }

    public NpgsqlTsVector VectorFr { get; set; }
}

public static class FtsEntityExtensions
{
    /// Performs the appropriate where statement for a direct full-text-search using the given language
    public static IQueryable<TEntity> FullTextSearch<TEntity>(this IQueryable<TEntity> query, string term,
        string language)
        where TEntity : IFtsEntity
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
    public static IQueryable<TEntity> FullTextSearch<TEntity, TFtsEntity>(this IQueryable<TEntity> query, DbSet<TFtsEntity> ftsEntities, string term,
        string language)  where TEntity : IHasId where TFtsEntity : class, IFtsEntity =>
        query.Where(hr => ftsEntities
            .FullTextSearch(term, language)
            .Select(x => x.OwnerId)
            .Contains(hr.Id));
}