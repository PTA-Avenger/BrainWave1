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
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"collaborationid\", \"taskid\", \"name\", \"description\" FROM collaborations WHERE \"taskid\" = @TaskID"
            : "SELECT CollaborationID, TaskID, Name, Description FROM Collaborations WHERE TaskID = @TaskID";
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
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"collaborationid\", \"taskid\", \"name\", \"description\" FROM collaborations WHERE \"collaborationid\" = @id"
            : "SELECT CollaborationID, TaskID, Name, Description FROM Collaborations WHERE CollaborationID = @id";
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
        cmd.CommandText = _provider == "postgres"
            ? @"INSERT INTO collaborations (\"taskid\", \"name\", \"description\")
                VALUES (@TaskID, @Name, @Description)
                RETURNING \"collaborationid\";"
            : @"INSERT INTO Collaborations (TaskID, Name, Description)
                VALUES (@TaskID, @Name, @Description);
                SELECT last_insert_rowid();";

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
        cmd.CommandText = _provider == "postgres"
            ? "DELETE FROM collaborations WHERE \"collaborationid\" = @id"
            : "DELETE FROM Collaborations WHERE CollaborationID = @id";
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

    public async Task<Collaboration?> GetCollaborationByPinAsync(string pin)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"collaborationid\", \"taskid\", \"collaboration_title\", \"collaboration_description\", \"invitepin\", \"created_date\", \"pin_expiry\", \"isactive\" FROM collaborations WHERE \"invitepin\" = @pin"
            : "SELECT CollaborationID, TaskID, Collaboration_Title, Collaboration_Description, InvitePin, Created_Date, Pin_Expiry, IsActive FROM Collaborations WHERE InvitePin = @pin";
        
        var param = cmd.CreateParameter();
        param.ParameterName = "@pin";
        param.Value = pin;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Collaboration
            {
                CollaborationID = reader.GetInt32(0),
                TaskID = reader.GetInt32(1),
                Collaboration_Title = reader.IsDBNull(2) ? null : reader.GetString(2),
                Collaboration_Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                InvitePin = reader.IsDBNull(4) ? null : reader.GetString(4),
                Created_Date = reader.IsDBNull(5) ? DateTime.UtcNow : reader.GetDateTime(5),
                Pin_Expiry = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                IsActive = reader.IsDBNull(7) ? true : reader.GetBoolean(7)
            };
        }
        return null;
    }

    public async Task<Collaboration?> GetCollaborationByIdAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"collaborationid\", \"taskid\", \"collaboration_title\", \"collaboration_description\", \"invitepin\", \"created_date\", \"pin_expiry\", \"isactive\" FROM collaborations WHERE \"collaborationid\" = @id"
            : "SELECT CollaborationID, TaskID, Collaboration_Title, Collaboration_Description, InvitePin, Created_Date, Pin_Expiry, IsActive FROM Collaborations WHERE CollaborationID = @id";
        
        var param = cmd.CreateParameter();
        param.ParameterName = "@id";
        param.Value = id;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Collaboration
            {
                CollaborationID = reader.GetInt32(0),
                TaskID = reader.GetInt32(1),
                Collaboration_Title = reader.IsDBNull(2) ? null : reader.GetString(2),
                Collaboration_Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                InvitePin = reader.IsDBNull(4) ? null : reader.GetString(4),
                Created_Date = reader.IsDBNull(5) ? DateTime.UtcNow : reader.GetDateTime(5),
                Pin_Expiry = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                IsActive = reader.IsDBNull(7) ? true : reader.GetBoolean(7)
            };
        }
        return null;
    }

    public async Task<List<string>> GetCollaboratorNamesAsync(int collaborationId)
    {
        var names = new List<string>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT u.\"f_name\" || ' ' || u.\"l_name\" FROM users u INNER JOIN usercollaborations uc ON u.\"userid\" = uc.\"userid\" WHERE uc.\"collaborationid\" = @collaborationId"
            : "SELECT u.F_Name + ' ' + u.L_Name FROM Users u INNER JOIN UserCollaborations uc ON u.UserID = uc.UserID WHERE uc.CollaborationID = @collaborationId";
        
        var param = cmd.CreateParameter();
        param.ParameterName = "@collaborationId";
        param.Value = collaborationId;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            names.Add(reader.GetString(0));
        }
        return names;
    }

    public async Task<bool> IsUserCollaboratorAsync(int collaborationId, int userId)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT COUNT(*) FROM usercollaborations WHERE \"collaborationid\" = @collaborationId AND \"userid\" = @userId"
            : "SELECT COUNT(*) FROM UserCollaborations WHERE CollaborationID = @collaborationId AND UserID = @userId";
        
        cmd.Parameters.Add(CreateParameter(cmd, "@collaborationId", collaborationId));
        cmd.Parameters.Add(CreateParameter(cmd, "@userId", userId));

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }

    public async Task AddUserToCollaborationAsync(int collaborationId, int userId)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "INSERT INTO usercollaborations (\"collaborationid\", \"userid\", \"role\") VALUES (@collaborationId, @userId, @role)"
            : "INSERT INTO UserCollaborations (CollaborationID, UserID, Role) VALUES (@collaborationId, @userId, @role)";
        
        cmd.Parameters.Add(CreateParameter(cmd, "@collaborationId", collaborationId));
        cmd.Parameters.Add(CreateParameter(cmd, "@userId", userId));
        cmd.Parameters.Add(CreateParameter(cmd, "@role", "Member"));

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task RemoveUserFromCollaborationAsync(int collaborationId, int userId)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "DELETE FROM usercollaborations WHERE \"collaborationid\" = @collaborationId AND \"userid\" = @userId"
            : "DELETE FROM UserCollaborations WHERE CollaborationID = @collaborationId AND UserID = @userId";
        
        cmd.Parameters.Add(CreateParameter(cmd, "@collaborationId", collaborationId));
        cmd.Parameters.Add(CreateParameter(cmd, "@userId", userId));

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateCollaborationAsync(Collaboration collaboration)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "UPDATE collaborations SET \"collaboration_title\" = @title, \"collaboration_description\" = @description, \"invitepin\" = @pin, \"pin_expiry\" = @expiry WHERE \"collaborationid\" = @id"
            : "UPDATE Collaborations SET Collaboration_Title = @title, Collaboration_Description = @description, InvitePin = @pin, Pin_Expiry = @expiry WHERE CollaborationID = @id";
        
        cmd.Parameters.Add(CreateParameter(cmd, "@title", collaboration.Collaboration_Title ?? (object)DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@description", collaboration.Collaboration_Description ?? (object)DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@pin", collaboration.InvitePin ?? (object)DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@expiry", collaboration.Pin_Expiry ?? (object)DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@id", collaboration.CollaborationID));

        await cmd.ExecuteNonQueryAsync();
    }
}