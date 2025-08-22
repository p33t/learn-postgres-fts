using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace app.Pkg.Model;

/// General full-text-search entity associated with various primary entities.
/// This is separate to allow calculated text and
/// multiple language indexes without increasing overhead on primary entity operations.
[PrimaryKey(nameof(Id))]
public class Fts
{
    public string Id { get; set; } = string.Empty;

    public string OwnerId { get; set; } = string.Empty;

    // Do later if necessary. Only the original language for now.
    // public LanguageEnum Language { get; set; }

    /// Strongest weighted text
    public string TextA { get; set; } = string.Empty;

    public NpgsqlTsVector VectorEn { get; set; } = null!;

    public NpgsqlTsVector VectorFr { get; set; } = null!;
}