using System.Reflection;

namespace tests.infra;

public static class TestExtensions
{
    /// Calculates a unique name for the specified method
    public static string CalcTestName(this MethodBase? testMethod, int maxLength = 5)
    {
        var rt = testMethod!.ReflectedType!;
        var methodName = rt.Name;
        var char0 = methodName.IndexOf('<') + 1;
        var length = methodName.IndexOf('>') - char0;
        methodName = methodName.Substring(char0, length);
        var className = rt.DeclaringType!.Name;
        
        return $"{className}.{methodName}";
    }
}