using BrainWave.API.Entities;
using Microsoft.Data.Sqlite;
using Npgsql;
using System.Data.Common;

public class UserRepository
{
    private readonly string _connectionString;
    private readonly string _provider;

    public UserRepository(IConfiguration config)
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

    public async Task<List<User>> GetUsersAsync()
    {
        var users = new List<User>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"userid\", \"f_name\", \"l_name\", \"email\", \"password_hash\", \"role\" FROM \"User\""
            : "SELECT UserID, F_Name, L_Name, Email, Password_Hash, Role FROM Users";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                UserID = reader.GetInt32(0),
                F_Name = reader.GetString(1),
                L_Name = reader.GetString(2),
                Email = reader.GetString(3),
                Password_Hash = reader.GetString(4),
                Role = reader.IsDBNull(5) ? null : reader.GetString(5)
            });
        }
        return users;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"userid\", \"f_name\", \"l_name\", \"email\", \"password_hash\", \"role\" FROM \"User\" WHERE \"userid\" = @id"
            : "SELECT UserID, F_Name, L_Name, Email, Password_Hash, Role FROM Users WHERE UserID = @id";
        cmd.Parameters.Add(CreateParameter(cmd, "@id", id));
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                UserID = reader.GetInt32(0),
                F_Name = reader.GetString(1),
                L_Name = reader.GetString(2),
                Email = reader.GetString(3),
                Password_Hash = reader.GetString(4),
                Role = reader.IsDBNull(5) ? null : reader.GetString(5)
            };
        }
        return null;
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT COUNT(1) FROM \"User\" WHERE \"email\" = @Email"
            : "SELECT COUNT(1) FROM Users WHERE Email = @Email";
        cmd.Parameters.Add(CreateParameter(cmd, "@Email", email));
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }

    public async Task AddUserAsync(User user)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"INSERT INTO \"User\" (\"f_name\", \"l_name\", \"email\", \"password_hash\", \"role\")
                VALUES (@F_Name, @L_Name, @Email, @Password_Hash, @Role)
                RETURNING \"userid\";"
            : @"INSERT INTO Users (F_Name, L_Name, Email, Password_Hash, Role)
                VALUES (@F_Name, @L_Name, @Email, @Password_Hash, @Role);
                SELECT last_insert_rowid();";

        cmd.Parameters.Add(CreateParameter(cmd, "@F_Name", user.F_Name));
        cmd.Parameters.Add(CreateParameter(cmd, "@L_Name", user.L_Name));
        cmd.Parameters.Add(CreateParameter(cmd, "@Email", user.Email));
        cmd.Parameters.Add(CreateParameter(cmd, "@Password_Hash", user.Password_Hash));
        cmd.Parameters.Add(CreateParameter(cmd, "@Role", (object?)user.Role ?? DBNull.Value));

        if (_provider == "postgres")
        {
            var result = await cmd.ExecuteScalarAsync();
            user.UserID = Convert.ToInt32(result);
        }
        else
        {
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = "SELECT last_insert_rowid();";
            var result = await cmd.ExecuteScalarAsync();
            user.UserID = Convert.ToInt32(result);
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"UPDATE \"User\"
                SET \"f_name\" = @F_Name,
                    \"l_name\" = @L_Name,
                    \"role\" = @Role
                WHERE \"userid\" = @UserID"
            : @"UPDATE Users
                SET F_Name = @F_Name,
                    L_Name = @L_Name,
                    Role = @Role
                WHERE UserID = @UserID";
        cmd.Parameters.Add(CreateParameter(cmd, "@F_Name", user.F_Name));
        cmd.Parameters.Add(CreateParameter(cmd, "@L_Name", user.L_Name));
        cmd.Parameters.Add(CreateParameter(cmd, "@Role", (object?)user.Role ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@UserID", user.UserID));
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "DELETE FROM \"User\" WHERE \"userid\" = @UserID"
            : "DELETE FROM Users WHERE UserID = @UserID";
        cmd.Parameters.Add(CreateParameter(cmd, "@UserID", userId));
        await cmd.ExecuteNonQueryAsync();
    }

    private static DbParameter CreateParameter(DbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        return param;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? "SELECT \"userid\", \"f_name\", \"l_name\", \"email\", \"password_hash\", \"role\", \"profile_picture\" FROM \"User\" WHERE \"email\" = @Email"
            : "SELECT UserID, F_Name, L_Name, Email, Password_Hash, Role, Profile_Picture FROM Users WHERE Email = @Email";
        cmd.Parameters.Add(CreateParameter(cmd, "@Email", email));

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                UserID = reader.GetInt32(0),
                F_Name = reader.GetString(1),
                L_Name = reader.GetString(2),
                Email = reader.GetString(3),
                Password_Hash = reader.GetString(4),
                Role = reader.GetString(5),
                Profile_Picture = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
        }
        return null;
    }

}
