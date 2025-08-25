namespace app.Pkg.Model;

public interface IHasFtses
{
    /// The full text search translations
    public List<Fts>? Ftses { get; set; }

    /// Returns the text for the highest weighted full-text-search field
    public string CalcTextA();
}

public static class HasFtsesExtensions
{
    /// Updates Full Text Search indexes (V2)
    public static void UpdateFtses(this IHasFtses hasFts)
    {
        // TODO: Need to check if '.Include(x => x.Ftses)' has been missed

        var fts = hasFts.Ftses?.FirstOrDefault();
        if (fts == null)
        {
            fts = new Fts { Id = Guid.NewGuid().ToString() };
            if (hasFts.Ftses == null)
            {
                hasFts.Ftses = new List<Fts>();
            }

            hasFts.Ftses.Add(fts);
        }

        fts.TextA = hasFts.CalcTextA();
    }
}