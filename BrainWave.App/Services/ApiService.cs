using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BrainWave.App.Models;


namespace BrainWave.App.Services;


public class ApiService
{
    private readonly HttpClient _http;
    private readonly AuthService _auth;
    private readonly LocalDatabase? _local; // optional for offline-first


    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };


    public ApiService(AuthService auth, LocalDatabase? local = null)
    {
        _auth = auth;
        _local = local;
        _http = new HttpClient { BaseAddress = new Uri(Constants.ApiBaseUrl + "/") };
    }


    private void AttachAuth()
    {
        _http.DefaultRequestHeaders.Authorization = _auth.Token != null
            ? new AuthenticationHeaderValue("Bearer", _auth.Token)
            : null;
    }

    // --- Auth ---
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest req)
    {
        var res = await _http.PostAsync("Auth/register", new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AuthResponse>(json, _jsonOpts);
    }

    public async Task<string?> LoginAsync(LoginRequest req)
    {
        var res = await _http.PostAsync("Auth/login", new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("token", out var t) ? t.GetString() : null;
    }

    // --- User ---
    public async Task<List<AuthResponse>?> GetUsersAsync()
    {
        AttachAuth();
        var res = await _http.GetAsync("User");
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<AuthResponse>>(json, _jsonOpts);
    }

    public async Task<AuthResponse?> GetUserAsync(int id)
    {
        AttachAuth();
        var res = await _http.GetAsync($"User/{id}");
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AuthResponse>(json, _jsonOpts);
    }

    public async Task<AuthResponse?> CreateUserAsync(AuthResponse user)
    {
        AttachAuth();
        var res = await _http.PostAsync("User", new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AuthResponse>(json, _jsonOpts);
    }

    public async Task<bool> UpdateUserAsync(int id, AuthResponse user)
    {
        AttachAuth();
        var res = await _http.PutAsync($"User/{id}", new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        AttachAuth();
        var res = await _http.DeleteAsync($"User/{id}");
        return res.IsSuccessStatusCode;
    }

    // --- Task ---
    public async Task<List<TaskDto>?> GetTasksAsync(int userId)
    {
        AttachAuth();
        try
        {
            var res = await _http.GetAsync($"Task/{userId}");
            if (!res.IsSuccessStatusCode) throw new HttpRequestException();
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TaskDto>>(json, _jsonOpts);
        }
        catch
        {
            if (_local != null)
            {
                var locals = await _local.GetTasksAsync(userId);
                return locals.Select(t => new TaskDto
                {
                    TaskID = t.RemoteTaskID ?? 0,
                    UserID = t.UserID,
                    Title = t.Title,
                    Description = t.Description,
                    Task_Status = t.Task_Status,
                    Priority_Level = t.Priority_Level,
                    Due_Date = t.Due_DateUtc.HasValue ? DateOnly.FromDateTime(t.Due_DateUtc.Value) : null
                }).ToList();
            }
            return null;
        }
    }

    public async Task<TaskDto?> CreateTaskAsync(TaskDto task)
    {
        AttachAuth();
        try
        {
            var res = await _http.PostAsync("Task", new StringContent(JsonSerializer.Serialize(task), Encoding.UTF8, "application/json"));
            if (!res.IsSuccessStatusCode) throw new HttpRequestException();
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TaskDto>(json, _jsonOpts);
        }
        catch
        {
            if (_local != null)
            {
                var local = new LocalTask
                {
                    RemoteTaskID = null,
                    UserID = task.UserID,
                    Title = task.Title,
                    Description = task.Description,
                    Task_Status = task.Task_Status,
                    Priority_Level = task.Priority_Level,
                    Due_DateUtc = task.Due_Date?.ToDateTime(TimeOnly.MinValue),
                    IsDirty = true,
                    UpdatedAtUtc = DateTime.UtcNow
                };
                await _local.InsertTaskAsync(local);
                return new TaskDto
                {
                    TaskID = 0,
                    UserID = local.UserID,
                    Title = local.Title,
                    Description = local.Description,
                    Task_Status = local.Task_Status,
                    Priority_Level = local.Priority_Level,
                    Due_Date = task.Due_Date
                };
            }
            return null;
        }
    }

    public async Task<bool> UpdateTaskAsync(int id, TaskDto task)
    {
        AttachAuth();
        try
        {
            var res = await _http.PutAsync($"Task/{id}", new StringContent(JsonSerializer.Serialize(task), Encoding.UTF8, "application/json"));
            return res.IsSuccessStatusCode;
        }
        catch
        {
            if (_local != null)
            {
                // best-effort: find local by RemoteTaskID
                var userId = task.UserID;
                var locals = await _local.GetTasksAsync(userId);
                var local = locals.FirstOrDefault(t => t.RemoteTaskID == id);
                if (local != null)
                {
                    local.Title = task.Title;
                    local.Description = task.Description;
                    local.Task_Status = task.Task_Status;
                    local.Priority_Level = task.Priority_Level;
                    local.Due_DateUtc = task.Due_Date?.ToDateTime(TimeOnly.MinValue);
                    local.IsDirty = true;
                    local.UpdatedAtUtc = DateTime.UtcNow;
                    await _local.UpdateTaskAsync(local);
                    return true;
                }
            }
            return false;
        }
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        AttachAuth();
        try
        {
            var res = await _http.DeleteAsync($"Task/{id}");
            return res.IsSuccessStatusCode;
        }
        catch
        {
            if (_local != null && _auth.CurrentUserId.HasValue)
            {
                var locals = await _local.GetTasksAsync(_auth.CurrentUserId.Value);
                var local = locals.FirstOrDefault(t => t.RemoteTaskID == id);
                if (local != null)
                {
                    local.IsDeleted = true;
                    local.IsDirty = true;
                    local.UpdatedAtUtc = DateTime.UtcNow;
                    await _local.UpdateTaskAsync(local);
                    return true;
                }
            }
            return false;
        }
    }

    // --- Collaboration ---
    public async Task<List<CollaborationDto>?> GetCollaborationsAsync(int taskId)
    {
        AttachAuth();
        var res = await _http.GetAsync($"Collaboration/{taskId}");
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<CollaborationDto>>(json, _jsonOpts);
    }

    public async Task<CollaborationDto?> CreateCollaborationAsync(CollaborationDto collab)
    {
        AttachAuth();
        var res = await _http.PostAsync("Collaboration", new StringContent(JsonSerializer.Serialize(collab), Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CollaborationDto>(json, _jsonOpts);
    }

    public async Task<bool> AddUserToCollaborationAsync(int collaborationId, int userId, string role)
    {
        AttachAuth();
        var res = await _http.PostAsync($"Collaboration/add-user/{collaborationId}/{userId}?role={Uri.EscapeDataString(role)}", null);
        return res.IsSuccessStatusCode;
    }

    // --- Reminder ---
    public async Task<List<ReminderDto>?> GetRemindersAsync(int taskId)
    {
        AttachAuth();
        var res = await _http.GetAsync($"Reminder/{taskId}");
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ReminderDto>>(json, _jsonOpts);
    }

    public async Task<ReminderDto?> CreateReminderAsync(ReminderDto reminder)
    {
        AttachAuth();
        var res = await _http.PostAsync("Reminder", new StringContent(JsonSerializer.Serialize(reminder), Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ReminderDto>(json, _jsonOpts);
    }

    public async Task<bool> DeleteReminderAsync(int id)
    {
        AttachAuth();
        var res = await _http.DeleteAsync($"Reminder/{id}");
        return res.IsSuccessStatusCode;
    }

    // --- Export ---
    public async Task<List<ExportDto>?> GetExportsAsync(int userId)
    {
        AttachAuth();
        var res = await _http.GetAsync($"Export/{userId}");
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ExportDto>>(json, _jsonOpts);
    }

    public async Task<ExportDto?> CreateExportAsync(ExportDto export)
    {
        AttachAuth();
        var res = await _http.PostAsync("Export", new StringContent(JsonSerializer.Serialize(export), Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ExportDto>(json, _jsonOpts);
    }

    // --- Badge ---
    public async Task<List<BadgeDto>?> GetBadgesAsync()
    {
        AttachAuth();
        var res = await _http.GetAsync("Badge");
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<BadgeDto>>(json, _jsonOpts);
    }

    public async Task<BadgeDto?> CreateBadgeAsync(BadgeDto badge)
    {
        AttachAuth();
        var res = await _http.PostAsync("Badge", new StringContent(JsonSerializer.Serialize(badge), Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BadgeDto>(json, _jsonOpts);
    }

    public async Task<bool> AssignBadgeAsync(int userId, int badgeId)
    {
        AttachAuth();
        var res = await _http.PostAsync($"Badge/assign/{userId}/{badgeId}", null);
        return res.IsSuccessStatusCode;
    }
}