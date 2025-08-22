namespace app.Pkg.Model;

public interface IHasFtses
{
    /// The full text search translations
    public List<Fts>? Ftses { get; set; }
}