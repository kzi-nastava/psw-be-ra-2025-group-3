namespace Explorer.BuildingBlocks.Infrastructure.Database;

public static class DbConnectionStringBuilder
{
    public static string Build(string schemaName)
    {
        var server = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
        var database = Environment.GetEnvironmentVariable("DATABASE_SCHEMA") ?? "explorer-v1";
        var schema = Environment.GetEnvironmentVariable("DATABASE_SCHEMA_NAME") ?? schemaName;
        var user = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "root";
        var pooling = Environment.GetEnvironmentVariable("DATABASE_POOLING") ?? "true";

        return
            $"Server={server};Port={port};Database={database};SearchPath={schema};User ID={user};Password={password};Pooling={pooling};";
    }
}