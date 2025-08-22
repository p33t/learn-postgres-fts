using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace app.Pkg.Model;

[PrimaryKey(nameof(Id))]
public class HotelReview2Fts : IFtsEntity
{
    public string Id { get; set; } = string.Empty;

    public string OwnerId { get; set; } = string.Empty;

    // Do later if necessary. Only the original language for now.
    // public LanguageEnum Language { get; set; }

    /// Strongest weighted text
    public string TextA { get; set; } = string.Empty;

    public NpgsqlTsVector VectorEn { get; set; } = null!;

    public NpgsqlTsVector VectorFr { get; set; } = null!;
    
    public HotelReview2? Owner { get; set; } = null;
}