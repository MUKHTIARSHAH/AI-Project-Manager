using Npgsql;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: AIPM.DbProvision <verify|create-database|inspect> <connection-string> [database-name]");
    return 1;
}

var command = args[0];
var connectionString = args[1];

return command switch
{
    "verify" => await VerifyAsync(connectionString),
    "create-database" when args.Length >= 3 => await CreateDatabaseAsync(connectionString, args[2]),
    "inspect" => await InspectAsync(connectionString),
    _ => UnknownCommand(command)
};

static int UnknownCommand(string command)
{
    Console.Error.WriteLine($"Unknown command: {command}");
    return 1;
}

static async Task<int> VerifyAsync(string connectionString)
{
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    await using var command = new NpgsqlCommand("SELECT version()", connection);
    var version = (string?)await command.ExecuteScalarAsync();
    Console.WriteLine($"Connected. PostgreSQL version: {version}");
    return 0;
}

static async Task<int> CreateDatabaseAsync(string adminConnectionString, string databaseName)
{
    await using var connection = new NpgsqlConnection(adminConnectionString);
    await connection.OpenAsync();

    await using var existsCommand = new NpgsqlCommand(
        "SELECT 1 FROM pg_database WHERE datname = @name",
        connection);
    existsCommand.Parameters.AddWithValue("name", databaseName);
    var exists = await existsCommand.ExecuteScalarAsync() is not null;
    if (exists)
    {
        Console.WriteLine($"Database '{databaseName}' already exists.");
        return 0;
    }

    await using var createCommand = new NpgsqlCommand(
        $"CREATE DATABASE \"{databaseName.Replace("\"", "\"\"", StringComparison.Ordinal)}\"",
        connection);
    await createCommand.ExecuteNonQueryAsync();
    Console.WriteLine($"Database '{databaseName}' created.");
    return 0;
}

static async Task<int> InspectAsync(string connectionString)
{
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();

    await PrintQueryAsync(connection, "SELECT current_database(), current_user");
    await PrintQueryAsync(connection, """
        SELECT "MigrationId"
        FROM "__EFMigrationsHistory"
        ORDER BY "MigrationId"
        """);

    await PrintQueryAsync(connection, """
        SELECT table_name
        FROM information_schema.tables
        WHERE table_schema = 'public'
          AND table_type = 'BASE TABLE'
        ORDER BY table_name
        """);

    await PrintQueryAsync(connection, """
        SELECT tc.table_name, tc.constraint_name, kcu.column_name, ccu.table_name AS foreign_table_name
        FROM information_schema.table_constraints AS tc
        JOIN information_schema.key_column_usage AS kcu
          ON tc.constraint_name = kcu.constraint_name AND tc.table_schema = kcu.table_schema
        JOIN information_schema.constraint_column_usage AS ccu
          ON ccu.constraint_name = tc.constraint_name AND ccu.table_schema = tc.table_schema
        WHERE tc.constraint_type = 'FOREIGN KEY'
          AND tc.table_schema = 'public'
        ORDER BY tc.table_name, tc.constraint_name
        """);

    await PrintQueryAsync(connection, """
        SELECT tablename, indexname
        FROM pg_indexes
        WHERE schemaname = 'public'
        ORDER BY tablename, indexname
        """);

    return 0;
}

static async Task PrintQueryAsync(NpgsqlConnection connection, string sql)
{
    Console.WriteLine();
    Console.WriteLine(sql);
    await using var command = new NpgsqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();
    var fieldCount = reader.FieldCount;
    while (await reader.ReadAsync())
    {
        var values = new string[fieldCount];
        for (var i = 0; i < fieldCount; i++)
        {
            values[i] = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i)?.ToString() ?? "NULL";
        }

        Console.WriteLine(string.Join(" | ", values));
    }
}
