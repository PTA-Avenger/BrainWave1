using BrainWave.Api.Entities;
using BrainWave.API.Entities;
using Microsoft.Data.Sqlite;
using Npgsql;
using System.Data.Common;

public class ReminderRepository
{
    private readonly string _connectionString;
    private readonly string _provider;

    public ReminderRepository(IConfiguration config)
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

    public async Task<List<Reminder>> GetRemindersByTaskIdAsync(int taskId)
    {
        var reminders = new List<Reminder>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"reminderid\", \"taskid\", \"reminder_type\", \"notify_time\" FROM reminders WHERE \"taskid\" = @TaskID"
            : "SELECT ReminderID, TaskID, Reminder_Type, Notify_Time FROM Reminders WHERE TaskID = @TaskID";
        cmd.Parameters.Add(CreateParameter(cmd, "@TaskID", taskId));
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            reminders.Add(new Reminder
            {
                ReminderID = reader.GetInt32(0),
                TaskID = reader.GetInt32(1),
                Reminder_Type = reader.GetString(2),
                Notify_Time = reader.GetDateTime(3)
            });
        }
        return reminders;
    }

    public async Task<Reminder?> GetReminderByIdAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"reminderid\", \"taskid\", \"reminder_type\", \"notify_time\" FROM reminders WHERE \"reminderid\" = @id"
            : "SELECT ReminderID, TaskID, Reminder_Type, Notify_Time FROM Reminders WHERE ReminderID = @id";
        cmd.Parameters.Add(CreateParameter(cmd, "@id", id));
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Reminder
            {
                ReminderID = reader.GetInt32(0),
                TaskID = reader.GetInt32(1),
                Reminder_Type = reader.GetString(2),
                Notify_Time = reader.GetDateTime(3)
            };
        }
        return null;
    }

    public async Task AddReminderAsync(Reminder reminder)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"INSERT INTO reminders (\"taskid\", \"reminder_type\", \"notify_time\")
                VALUES (@TaskID, @Reminder_Type, @Notify_Time)
                RETURNING \"reminderid\";"
            : @"INSERT INTO Reminders (TaskID, Reminder_Type, Notify_Time)
                VALUES (@TaskID, @Reminder_Type, @Notify_Time);
                SELECT last_insert_rowid();";

        cmd.Parameters.Add(CreateParameter(cmd, "@TaskID", reminder.TaskID));
        cmd.Parameters.Add(CreateParameter(cmd, "@Reminder_Type", reminder.Reminder_Type));
        cmd.Parameters.Add(CreateParameter(cmd, "@Notify_Time", reminder.Notify_Time));

        if (_provider == "postgres")
        {
            var result = await cmd.ExecuteScalarAsync();
            reminder.ReminderID = Convert.ToInt32(result);
        }
        else
        {
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = "SELECT last_insert_rowid();";
            var result = await cmd.ExecuteScalarAsync();
            reminder.ReminderID = Convert.ToInt32(result);
        }
    }

    public async Task DeleteReminderAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "DELETE FROM reminders WHERE \"reminderid\" = @id"
            : "DELETE FROM Reminders WHERE ReminderID = @id";
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