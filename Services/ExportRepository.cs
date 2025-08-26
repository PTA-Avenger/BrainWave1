using BrainWave.Api.Entities;
using Microsoft.Data.Sqlite;
using Npgsql;
using System.Data.Common;

public class ExportRepository
{
    private readonly string _connectionString;
    private readonly string _provider;

    public ExportRepository(IConfiguration config)
    {
        _provider = config.GetValue<string>("UseProvider")?.ToLower() ?? "sqlite";
        _connectionString = _provider == "postgres"
            ? config.GetConnectionString("Postgres")
            : config.GetConnectionString("Sqlite");
    }

    private DbConnection CreateConnection()
    {
        return _provider == "postgres"
            ? new NpgsqlConnection(_connectionString)
            : new SqliteConnection(_connectionString);
    }

    public async Task<List<Export>> GetExportsByUserIdAsync(int userId)
    {
        var exports = new List<Export>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT ExportID, UserID, TaskID, Export_Format, Date_Requested FROM Exports WHERE UserID = @UserID";
        cmd.Parameters.Add(CreateParameter(cmd, "@UserID", userId));

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            exports.Add(new Export
            {
                ExportID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                TaskID = reader.GetInt32(2),
                Export_Format = reader.GetString(3),
                Date_Requested = reader.GetDateTime(4)
            });
        }
        return exports;
    }

    public async Task AddExportAsync(Export export)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Exports (UserID, TaskID, Export_Format, Date_Requested)
            VALUES (@UserID, @TaskID, @Export_Format, @Date_Requested);
            " + (_provider == "postgres" ? "RETURNING ExportID;" : "SELECT last_insert_rowid();");

        cmd.Parameters.Add(CreateParameter(cmd, "@UserID", export.UserID));
        cmd.Parameters.Add(CreateParameter(cmd, "@TaskID", export.TaskID));
        cmd.Parameters.Add(CreateParameter(cmd, "@Export_Format", export.Export_Format));
        cmd.Parameters.Add(CreateParameter(cmd, "@Date_Requested", export.Date_Requested));

        if (_provider == "postgres")
        {
            var result = await cmd.ExecuteScalarAsync();
            export.ExportID = Convert.ToInt32(result);
        }
        else
        {
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = "SELECT last_insert_rowid();";
            var result = await cmd.ExecuteScalarAsync();
            export.ExportID = Convert.ToInt32(result);
        }
    }

    private static DbParameter CreateParameter(DbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        return param;
    }
}
