namespace app.Pkg.Support;

public interface IDbLogTo
{
    /// Outputs a line of text to the desired destination
    public void WriteLine(string line);
}