using BrainWave.API.Entities;
using Microsoft.Data.Sqlite;
using Npgsql;
using System.Data.Common;

public class TasksRepository
{
    private readonly string _connectionString;
    private readonly string _provider;

    public TasksRepository(IConfiguration config)
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

    public async Task<List<Tasks>> GetTasksByUserIdAsync(int userId)
    {
        var tasks = new List<Tasks>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT TaskID, UserID, Title, Description, Due_Date, Task_Status, Priority_Level FROM Tasks WHERE UserID = @userId";
        var param = cmd.CreateParameter();
        param.ParameterName = "@userId";
        param.Value = userId;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(new Tasks
            {
                TaskID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                Title = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Due_Date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Task_Status = reader.IsDBNull(5) ? null : reader.GetString(5),
                Priority_Level = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }
        return tasks;
    }

    public async Task<Tasks?> GetTaskByIdAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT TaskID, UserID, Title, Description, Due_Date, Task_Status, Priority_Level FROM Tasks WHERE TaskID = @id";
        var param = cmd.CreateParameter();
        param.ParameterName = "@id";
        param.Value = id;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Tasks
            {
                TaskID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                Title = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Due_Date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Task_Status = reader.IsDBNull(5) ? null : reader.GetString(5),
                Priority_Level = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
        }
        return null;
    }

    public async Task AddTaskAsync(Tasks task)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO Tasks (UserID, Title, Description, Due_Date, Task_Status, Priority_Level)
            VALUES (@UserID, @Title, @Description, @Due_Date, @Task_Status, @Priority_Level);
            " + (_provider == "postgres" ? "RETURNING TaskID;" : "SELECT last_insert_rowid();");

        cmd.Parameters.Add(CreateParameter(cmd, "@UserID", task.UserID));
        cmd.Parameters.Add(CreateParameter(cmd, "@Title", task.Title));
        cmd.Parameters.Add(CreateParameter(cmd, "@Description", (object?)task.Description ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Due_Date", (object?)task.Due_Date ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Task_Status", (object?)task.Task_Status ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Priority_Level", (object?)task.Priority_Level ?? DBNull.Value));

        if (_provider == "postgres")
        {
            var result = await cmd.ExecuteScalarAsync();
            task.TaskID = Convert.ToInt32(result);
        }
        else
        {
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = "SELECT last_insert_rowid();";
            var result = await cmd.ExecuteScalarAsync();
            task.TaskID = Convert.ToInt32(result);
        }
    }

    public async Task UpdateTaskAsync(Tasks task)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        UPDATE Tasks
        SET Title = @Title,
            Description = @Description,
            Due_Date = @Due_Date,
            Task_Status = @Task_Status,
            Priority_Level = @Priority_Level
        WHERE TaskID = @TaskID";
        cmd.Parameters.Add(CreateParameter(cmd, "@Title", task.Title));
        cmd.Parameters.Add(CreateParameter(cmd, "@Description", (object?)task.Description ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Due_Date", (object?)task.Due_Date ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Task_Status", (object?)task.Task_Status ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Priority_Level", (object?)task.Priority_Level ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@TaskID", task.TaskID));
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteTaskAsync(Tasks task)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Tasks WHERE TaskID = @TaskID";
        cmd.Parameters.Add(CreateParameter(cmd, "@TaskID", task.TaskID));
        await cmd.ExecuteNonQueryAsync();
    }

    private static DbParameter CreateParameter(DbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        return param;
    }

    public async Task<List<Tasks>> GetAllTasks()
    {
        var tasks = new List<Tasks>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT TaskID, UserID, Title, Description, Due_Date, Task_Status, Priority_Level FROM Tasks";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(new Tasks
            {
                TaskID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                Title = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Due_Date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Task_Status = reader.IsDBNull(5) ? null : reader.GetString(5),
                Priority_Level = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }
        return tasks;
    }
}