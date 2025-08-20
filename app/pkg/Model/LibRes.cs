using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace app.pkg.Model;

/// Library Resource that needs to be full text searchable
[PrimaryKey(nameof(Id))]
public class LibRes
{
    public string Id { get; set; } = string.Empty;
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Abstract { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "text")]
    public LanguageEnum Language { get; set; }
}