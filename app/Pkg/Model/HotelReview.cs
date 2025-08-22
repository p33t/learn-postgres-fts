using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace app.Pkg.Model;

[PrimaryKey(nameof(Id))]
public class HotelReview
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

    public string Id { get; set; } = string.Empty;
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Text { get; set; } = string.Empty;
    
    [Required]
    [AllowedValues(1, 2, 3, 4, 5)]
    public int Rating { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public string Property { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "text")]
    public LanguageEnum Language { get; set; }
    
    public NpgsqlTsVector VectorEn { get; set; } = null!;

    // public NpgsqlTsVector VectorFr { get; set; } = null!;
}