namespace app.Pkg.Support;

/// Allows dynamic assignment of where to write Db query logs
public class DbLogTo : IDbLogTo
{
    private Action<string>? _writeLine;
    
    public void WriteLine(string line)
    {
        if (_writeLine != null)
        {
            _writeLine.Invoke(line);
        }
    }

    public void AssignWriteLine(Action<string> writeLine)
    {
        _writeLine = writeLine;
    }

    public void ClearWriteLine()
    {
        _writeLine = null;
    }
}