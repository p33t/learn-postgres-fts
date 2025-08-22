using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace app.Pkg.Model;

[PrimaryKey(nameof(Id))]
public class HotelReviewBase
{
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
}