using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal;

namespace app.Pkg;

// Ignore warnings about using internals of API
#pragma warning disable EF1001

/// Use lowercase table and field names for simpler SQL
public class SqlGenerationHelper(RelationalSqlGenerationHelperDependencies dependencies) : NpgsqlSqlGenerationHelper(dependencies)
{
    public override void DelimitIdentifier(StringBuilder builder, string identifier)
    {
        base.DelimitIdentifier(builder, identifier.ToLowerInvariant());
    }

    public override void DelimitIdentifier(StringBuilder builder, string name, string? schema)
    {
        base.DelimitIdentifier(builder, name.ToLowerInvariant(), schema?.ToLowerInvariant());
    }

    public override string DelimitIdentifier(string identifier)
    {
        return base.DelimitIdentifier(identifier.ToLowerInvariant());
    }

    public override string DelimitIdentifier(string name, string? schema)
    {
        return base.DelimitIdentifier(name.ToLowerInvariant(), schema?.ToLowerInvariant());
    }

    public override bool Equals(object? obj)
    {
        // All instances of this object are equivalent to avoid ManyServiceProvidersCreatedWarning
        return ReferenceEquals(this, obj) || obj is SqlGenerationHelper;
    }

    public override int GetHashCode()
    {
        return nameof(SqlGenerationHelper).GetHashCode();
    }
}
#pragma warning restore EF1001
