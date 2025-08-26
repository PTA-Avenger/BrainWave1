using BrainWave.App.Models;

namespace BrainWave.App.Services;

public class SyncService
{
	private readonly LocalDatabase _local;
	private readonly ApiService _api;
	private readonly AuthService _auth;
	private bool _running;

	public SyncService(LocalDatabase local, ApiService api, AuthService auth)
	{
		_local = local;
		_api = api;
		_auth = auth;
	}

	public async Task StartAsync(CancellationToken token = default)
	{
		_running = true;
		await _local.InitializeAsync();
		_ = Task.Run(() => LoopAsync(token), token);
	}

	private async Task LoopAsync(CancellationToken token)
	{
		while (_running && !token.IsCancellationRequested)
		{
			try
			{
				if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet && _auth.CurrentUserId.HasValue)
				{
					await PushTasksAsync(_auth.CurrentUserId.Value);
				}
			}
			catch
			{
				// intentionally ignore to keep loop resilient
			}
			await Task.Delay(TimeSpan.FromSeconds(10), token);
		}
	}

	private async Task PushTasksAsync(int userId)
	{
		var dirty = await _local.GetDirtyTasksAsync();
		foreach (var task in dirty)
		{
			if (task.IsDeleted)
			{
				if (task.RemoteTaskID.HasValue)
				{
					var ok = await _api.DeleteTaskAsync(task.RemoteTaskID.Value);
					if (ok)
					{
						await _local.DeleteTaskAsync(task);
					}
					continue;
				}
				await _local.DeleteTaskAsync(task);
				continue;
			}

			if (!task.RemoteTaskID.HasValue)
			{
				var dto = new TaskDto
				{
					UserID = task.UserID,
					Title = task.Title,
					Description = task.Description,
					Task_Status = task.Task_Status,
					Priority_Level = task.Priority_Level,
					Due_Date = task.Due_DateUtc.HasValue ? DateOnly.FromDateTime(task.Due_DateUtc.Value) : null
				};
				var created = await _api.CreateTaskAsync(dto);
				if (created != null)
				{
					task.RemoteTaskID = created.TaskID;
					task.IsDirty = false;
					await _local.UpdateTaskAsync(task);
				}
			}
			else
			{
				var dto = new TaskDto
				{
					TaskID = task.RemoteTaskID.Value,
					UserID = task.UserID,
					Title = task.Title,
					Description = task.Description,
					Task_Status = task.Task_Status,
					Priority_Level = task.Priority_Level,
					Due_Date = task.Due_DateUtc.HasValue ? DateOnly.FromDateTime(task.Due_DateUtc.Value) : null
				};
				var ok = await _api.UpdateTaskAsync(dto.TaskID, dto);
				if (ok)
				{
					task.IsDirty = false;
					await _local.UpdateTaskAsync(task);
				}
			}
		}
	}
}

