using System.Linq.Expressions;
using System.Reflection;
using app.Pkg.Model;
using Microsoft.EntityFrameworkCore;

namespace app.Pkg;

public static class Extensions
{
    private const string LEAN_PROP_NAME = nameof(HotelReview.LEAN);

    /// Convenience method to list elements of an IQueryable and
    /// 'Select' only the necessary fields by using a 'LEAN' static expression on the TEntity class (if available).
    /// See <see cref="HotelReview.LEAN"/>
    public static Task<List<TEntity>> ToListLeanAsync<TEntity>(this IQueryable<TEntity> query)
        where TEntity : class
    {
        var altQuery = query;
        var entityType = typeof(TEntity);
        if (entityType.GetField(LEAN_PROP_NAME, BindingFlags.Static | BindingFlags.Public) is FieldInfo leanField)
        {
            // lean expression is available
            Expression<Func<TEntity, TEntity>> expr = (Expression<Func<TEntity, TEntity>>) leanField.GetValue(null)!;
            altQuery = altQuery.Select(expr);
        }

        return altQuery.ToListAsync();
    }
}