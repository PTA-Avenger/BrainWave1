using Microsoft.Data.Sqlite;
using Npgsql;
using System.Data.Common;

public class DatabaseInitializer
{
    private readonly string _connectionString;
    private readonly string _provider;

    public DatabaseInitializer(IConfiguration config)
    {
        _provider = config.GetValue<string>("UseProvider")?.ToLower() ?? "sqlite";
        _connectionString = _provider == "postgres"
            ? config.GetConnectionString("Postgres")
            : config.GetConnectionString("Sqlite");
    }

    public async Task InitializeDatabaseAsync()
    {
        if (_provider == "sqlite")
        {
            await InitializeSqliteDatabaseAsync();
        }
        else if (_provider == "postgres")
        {
            await InitializePostgresDatabaseAsync();
        }
    }

    private async Task InitializeSqliteDatabaseAsync()
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        
        // Create Users table
        var createUsersTable = @"
            CREATE TABLE IF NOT EXISTS Users (
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                F_Name TEXT NOT NULL,
                L_Name TEXT NOT NULL,
                Email TEXT UNIQUE NOT NULL,
                Password_Hash TEXT NOT NULL,
                Role TEXT,
                Profile_Picture TEXT
            );";

        using (var cmd = new SqliteCommand(createUsersTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create Tasks table
        var createTasksTable = @"
            CREATE TABLE IF NOT EXISTS Tasks (
                TaskID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Description TEXT,
                Status TEXT NOT NULL,
                Priority TEXT NOT NULL,
                DueDate TEXT,
                CreatedDate TEXT NOT NULL,
                FOREIGN KEY (UserID) REFERENCES Users(UserID)
            );";

        using (var cmd = new SqliteCommand(createTasksTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create Reminders table
        var createRemindersTable = @"
            CREATE TABLE IF NOT EXISTS Reminders (
                ReminderID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER NOT NULL,
                Title TEXT NOT NULL,
                Description TEXT,
                ReminderTime TEXT NOT NULL,
                IsCompleted BOOLEAN NOT NULL DEFAULT 0,
                CreatedDate TEXT NOT NULL,
                FOREIGN KEY (UserID) REFERENCES Users(UserID)
            );";

        using (var cmd = new SqliteCommand(createRemindersTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create Badges table
        var createBadgesTable = @"
            CREATE TABLE IF NOT EXISTS Badges (
                BadgeID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                Icon TEXT
            );";

        using (var cmd = new SqliteCommand(createBadgesTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create UserBadges table
        var createUserBadgesTable = @"
            CREATE TABLE IF NOT EXISTS UserBadges (
                UserBadgeID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER NOT NULL,
                BadgeID INTEGER NOT NULL,
                EarnedDate TEXT NOT NULL,
                FOREIGN KEY (UserID) REFERENCES Users(UserID),
                FOREIGN KEY (BadgeID) REFERENCES Badges(BadgeID)
            );";

        using (var cmd = new SqliteCommand(createUserBadgesTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create Collaborations table
        var createCollaborationsTable = @"
            CREATE TABLE IF NOT EXISTS Collaborations (
                CollaborationID INTEGER PRIMARY KEY AUTOINCREMENT,
                TaskID INTEGER NOT NULL,
                UserID INTEGER NOT NULL,
                Role TEXT NOT NULL,
                CreatedDate TEXT NOT NULL,
                FOREIGN KEY (TaskID) REFERENCES Tasks(TaskID),
                FOREIGN KEY (UserID) REFERENCES Users(UserID)
            );";

        using (var cmd = new SqliteCommand(createCollaborationsTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create Exports table
        var createExportsTable = @"
            CREATE TABLE IF NOT EXISTS Exports (
                ExportID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID INTEGER NOT NULL,
                ExportType TEXT NOT NULL,
                FilePath TEXT NOT NULL,
                CreatedDate TEXT NOT NULL,
                FOREIGN KEY (UserID) REFERENCES Users(UserID)
            );";

        using (var cmd = new SqliteCommand(createExportsTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Insert some default badges
        var insertDefaultBadges = @"
            INSERT OR IGNORE INTO Badges (Name, Description, Icon) VALUES 
            ('First Task', 'Completed your first task', '🎯'),
            ('Task Master', 'Completed 10 tasks', '🏆'),
            ('Early Bird', 'Completed a task before 9 AM', '🌅'),
            ('Night Owl', 'Completed a task after 10 PM', '🦉'),
            ('Team Player', 'Collaborated on 5 tasks', '🤝'),
            ('Organizer', 'Created 20 tasks', '📋'),
            ('Reminder Pro', 'Set up 10 reminders', '⏰'),
            ('Export Expert', 'Exported data 5 times', '📊');";

        using (var cmd = new SqliteCommand(insertDefaultBadges, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task InitializePostgresDatabaseAsync()
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        
        // Create Users table
        var createUsersTable = @"
            CREATE TABLE IF NOT EXISTS Users (
                UserID SERIAL PRIMARY KEY,
                F_Name VARCHAR(100) NOT NULL,
                L_Name VARCHAR(100) NOT NULL,
                Email VARCHAR(255) UNIQUE NOT NULL,
                Password_Hash VARCHAR(255) NOT NULL,
                Role VARCHAR(50),
                Profile_Picture TEXT
            );";

        using (var cmd = new NpgsqlCommand(createUsersTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create Tasks table
        var createTasksTable = @"
            CREATE TABLE IF NOT EXISTS Tasks (
                TaskID SERIAL PRIMARY KEY,
                UserID INTEGER NOT NULL,
                Title VARCHAR(255) NOT NULL,
                Description TEXT,
                Status VARCHAR(50) NOT NULL,
                Priority VARCHAR(50) NOT NULL,
                DueDate TIMESTAMP,
                CreatedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserID) REFERENCES Users(UserID)
            );";

        using (var cmd = new NpgsqlCommand(createTasksTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create Reminders table
        var createRemindersTable = @"
            CREATE TABLE IF NOT EXISTS Reminders (
                ReminderID SERIAL PRIMARY KEY,
                UserID INTEGER NOT NULL,
                Title VARCHAR(255) NOT NULL,
                Description TEXT,
                ReminderTime TIMESTAMP NOT NULL,
                IsCompleted BOOLEAN NOT NULL DEFAULT FALSE,
                CreatedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserID) REFERENCES Users(UserID)
            );";

        using (var cmd = new NpgsqlCommand(createRemindersTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create Badges table
        var createBadgesTable = @"
            CREATE TABLE IF NOT EXISTS Badges (
                BadgeID SERIAL PRIMARY KEY,
                Name VARCHAR(100) NOT NULL,
                Description TEXT,
                Icon VARCHAR(255)
            );";

        using (var cmd = new NpgsqlCommand(createBadgesTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create UserBadges table
        var createUserBadgesTable = @"
            CREATE TABLE IF NOT EXISTS UserBadges (
                UserBadgeID SERIAL PRIMARY KEY,
                UserID INTEGER NOT NULL,
                BadgeID INTEGER NOT NULL,
                EarnedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserID) REFERENCES Users(UserID),
                FOREIGN KEY (BadgeID) REFERENCES Badges(BadgeID)
            );";

        using (var cmd = new NpgsqlCommand(createUserBadgesTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create Collaborations table
        var createCollaborationsTable = @"
            CREATE TABLE IF NOT EXISTS Collaborations (
                CollaborationID SERIAL PRIMARY KEY,
                TaskID INTEGER NOT NULL,
                UserID INTEGER NOT NULL,
                Role VARCHAR(50) NOT NULL,
                CreatedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (TaskID) REFERENCES Tasks(TaskID),
                FOREIGN KEY (UserID) REFERENCES Users(UserID)
            );";

        using (var cmd = new NpgsqlCommand(createCollaborationsTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Create Exports table
        var createExportsTable = @"
            CREATE TABLE IF NOT EXISTS Exports (
                ExportID SERIAL PRIMARY KEY,
                UserID INTEGER NOT NULL,
                ExportType VARCHAR(50) NOT NULL,
                FilePath TEXT NOT NULL,
                CreatedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserID) REFERENCES Users(UserID)
            );";

        using (var cmd = new NpgsqlCommand(createExportsTable, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }

        // Insert some default badges
        var insertDefaultBadges = @"
            INSERT INTO Badges (Name, Description, Icon) VALUES 
            ('First Task', 'Completed your first task', '🎯'),
            ('Task Master', 'Completed 10 tasks', '🏆'),
            ('Early Bird', 'Completed a task before 9 AM', '🌅'),
            ('Night Owl', 'Completed a task after 10 PM', '🦉'),
            ('Team Player', 'Collaborated on 5 tasks', '🤝'),
            ('Organizer', 'Created 20 tasks', '📋'),
            ('Reminder Pro', 'Set up 10 reminders', '⏰'),
            ('Export Expert', 'Exported data 5 times', '📊')
            ON CONFLICT DO NOTHING;";

        using (var cmd = new NpgsqlCommand(insertDefaultBadges, conn))
        {
            await cmd.ExecuteNonQueryAsync();
        }
    }
}