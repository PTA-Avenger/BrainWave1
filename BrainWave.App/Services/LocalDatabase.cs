using SQLite;
using BrainWave.App.Models;

namespace BrainWave.App.Services;

public class LocalDatabase
{
	private readonly SQLiteAsyncConnection _db;

	public LocalDatabase()
	{
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "brainwave_offline.db3");
		_db = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
	}

	public async Task InitializeAsync()
	{
		await _db.CreateTableAsync<LocalTask>();
	}

	// CRUD for LocalTask
	public Task<List<LocalTask>> GetTasksAsync(int userId) => _db.Table<LocalTask>().Where(t => t.UserID == userId && !t.IsDeleted).ToListAsync();
	public Task<LocalTask?> GetTaskAsync(int id) => _db.FindAsync<LocalTask>(id);
	public Task<int> InsertTaskAsync(LocalTask task) => _db.InsertAsync(task);
	public Task<int> UpdateTaskAsync(LocalTask task) => _db.UpdateAsync(task);
	public Task<int> DeleteTaskAsync(LocalTask task) => _db.DeleteAsync(task);

	public Task<List<LocalTask>> GetDirtyTasksAsync() => _db.Table<LocalTask>().Where(t => t.IsDirty || t.IsDeleted).OrderBy(t => t.UpdatedAtUtc).ToListAsync();
}

