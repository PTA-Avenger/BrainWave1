using BrainWave.Api.Entities;
using BrainWave.API.Entities;
using Microsoft.Data.Sqlite;
using Npgsql;
using System.Data.Common;

public class CollaborationRepository
{
    private readonly string _connectionString;
    private readonly string _provider;

    public CollaborationRepository(IConfiguration config)
    {
        _provider = config.GetValue<string>("UseProvider")?.ToLower() ?? "sqlite";
        _connectionString = _provider == "postgres"
            ? config.GetConnectionString("Postgres")
            : config.GetConnectionString("Sqlite");
    }

    private DbConnection CreateConnection()
    {
        if (_provider == "postgres")
            return new NpgsqlConnection(_connectionString);
        else
            return new SqliteConnection(_connectionString);
    }

    public async Task<List<Collaboration>> GetCollaborationsByTaskIdAsync(int taskId)
    {
        var collaborations = new List<Collaboration>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT CollaborationID, TaskID, Name, Description FROM Collaborations WHERE TaskID = @TaskID";
        cmd.Parameters.Add(CreateParameter(cmd, "@TaskID", taskId));
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            collaborations.Add(new Collaboration
            {
                CollaborationID = reader.GetInt32(0),
                TaskID = reader.GetInt32(1),
                Collaboration_Title = reader.GetString(2),
                Collaboration_Description = reader.IsDBNull(3) ? null : reader.GetString(3)
            });
        }
        return collaborations;
    }

    public async Task<Collaboration?> GetCollaborationByIdAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT CollaborationID, TaskID, Name, Description FROM Collaborations WHERE CollaborationID = @id";
        cmd.Parameters.Add(CreateParameter(cmd, "@id", id));
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Collaboration
            {
                CollaborationID = reader.GetInt32(0),
                TaskID = reader.GetInt32(1),
                Collaboration_Title = reader.GetString(2),
                Collaboration_Description = reader.IsDBNull(3) ? null : reader.GetString(3)
            };
        }
        return null;
    }

    public async Task AddCollaborationAsync(Collaboration collaboration)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Collaborations (TaskID, Name, Description)
            VALUES (@TaskID, @Name, @Description);
            " + (_provider == "postgres" ? "RETURNING CollaborationID;" : "SELECT last_insert_rowid();");

        cmd.Parameters.Add(CreateParameter(cmd, "@TaskID", collaboration.TaskID));
        cmd.Parameters.Add(CreateParameter(cmd, "@Name", collaboration.Collaboration_Title));
        cmd.Parameters.Add(CreateParameter(cmd, "@Description", (object?)collaboration.Collaboration_Description ?? DBNull.Value));

        if (_provider == "postgres")
        {
            var result = await cmd.ExecuteScalarAsync();
            collaboration.CollaborationID = Convert.ToInt32(result);
        }
        else
        {
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = "SELECT last_insert_rowid();";
            var result = await cmd.ExecuteScalarAsync();
            collaboration.CollaborationID = Convert.ToInt32(result);
        }
    }

    public async Task DeleteCollaborationAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Collaborations WHERE CollaborationID = @id";
        cmd.Parameters.Add(CreateParameter(cmd, "@id", id));
        await cmd.ExecuteNonQueryAsync();
    }

    private static DbParameter CreateParameter(DbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        return param;
    }
}