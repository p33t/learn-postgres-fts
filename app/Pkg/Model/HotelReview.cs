using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace app.Pkg.Model;

public class HotelReview : HotelReviewBase
{
    /// Takes only the necessary fields and leaves expensive, unnecessary fields uncopied
    public static readonly Expression<Func<HotelReview, HotelReview>> LEAN = x => new()
    {
        Id = x.Id,
        Title = x.Title,
        Text = x.Text,
        Rating = x.Rating,
        Language = x.Language,
        Property = x.Property,
        Date = x.Date
    };
    
    public NpgsqlTsVector VectorEn { get; set; } = null!;

    // public NpgsqlTsVector VectorFr { get; set; } = null!;
}