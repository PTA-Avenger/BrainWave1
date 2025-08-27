using BrainWave.Api.Entities;
using BrainWave.API.Entities;
using Microsoft.Data.Sqlite;
using Npgsql;
using System.Data.Common;

public class BadgeRepository
{
    private readonly string _connectionString;
    private readonly string _provider;

    public BadgeRepository(IConfiguration config)
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

    public async Task<List<Badge>> GetAllBadgesAsync()
    {
        var badges = new List<Badge>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"badgeid\", \"badge_type\", \"badge_description\" FROM badges"
            : "SELECT BadgeID, Badge_Type, Badge_Description FROM Badges";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            badges.Add(new Badge
            {
                BadgeID = reader.GetInt32(0),
                Badge_Type = reader.GetString(1),
                Badge_Description = reader.GetString(2)
            });
        }
        return badges;
    }

    public async Task<Badge?> GetBadgeByIdAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"badgeid\", \"badge_type\", \"badge_description\" FROM badges WHERE \"badgeid\" = @id"
            : "SELECT BadgeID, Badge_Type, Badge_Description FROM Badges WHERE BadgeID = @id";
        cmd.Parameters.Add(CreateParameter(cmd, "@id", id));
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Badge
            {
                BadgeID = reader.GetInt32(0),
                Badge_Type = reader.GetString(1),
                Badge_Description = reader.GetString(2)
            };
        }
        return null;
    }

    public async Task AddBadgeAsync(Badge badge)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"INSERT INTO badges (\"badge_type\", \"badge_description\")
                VALUES (@Badge_Type, @Badge_Description)
                RETURNING \"badgeid\";"
            : @"INSERT INTO Badges (Badge_Type, Badge_Description)
                VALUES (@Badge_Type, @Badge_Description);
                SELECT last_insert_rowid();";

        cmd.Parameters.Add(CreateParameter(cmd, "@Badge_Type", badge.Badge_Type));
        cmd.Parameters.Add(CreateParameter(cmd, "@Badge_Description", badge.Badge_Description));

        if (_provider == "postgres")
        {
            var result = await cmd.ExecuteScalarAsync();
            badge.BadgeID = Convert.ToInt32(result);
        }
        else
        {
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = "SELECT last_insert_rowid();";
            var result = await cmd.ExecuteScalarAsync();
            badge.BadgeID = Convert.ToInt32(result);
        }
    }

    public async Task<bool> AssignBadgeAsync(int userId, int badgeId)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"INSERT INTO userbadges (\"userid\", \"badgeid\", \"date_earned\")
                VALUES (@UserID, @BadgeID, @Date_Earned);"
            : @"INSERT INTO UserBadges (UserID, BadgeID, Date_Earned)
                VALUES (@UserID, @BadgeID, @Date_Earned);";

        cmd.Parameters.Add(CreateParameter(cmd, "@UserID", userId));
        cmd.Parameters.Add(CreateParameter(cmd, "@BadgeID", badgeId));
        cmd.Parameters.Add(CreateParameter(cmd, "@Date_Earned", DateTime.UtcNow));

        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private static DbParameter CreateParameter(DbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        return param;
    }
}
