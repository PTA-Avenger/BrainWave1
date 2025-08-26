using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BrainWave.App.Models;


namespace BrainWave.App.Services;


public class ApiService
{
    private readonly HttpClient _http;
    private readonly AuthService _auth;


    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };


    public ApiService(AuthService auth)
    {
        _auth = auth;
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
        var res = await _http.GetAsync($"Task/{userId}");
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TaskDto>>(json, _jsonOpts);
    }

    public async Task<TaskDto?> CreateTaskAsync(TaskDto task)
    {
        AttachAuth();
        var res = await _http.PostAsync("Task", new StringContent(JsonSerializer.Serialize(task), Encoding.UTF8, "application/json"));
        if (!res.IsSuccessStatusCode) return null;
        var json = await res.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TaskDto>(json, _jsonOpts);
    }

    public async Task<bool> UpdateTaskAsync(int id, TaskDto task)
    {
        AttachAuth();
        var res = await _http.PutAsync($"Task/{id}", new StringContent(JsonSerializer.Serialize(task), Encoding.UTF8, "application/json"));
        return res.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        AttachAuth();
        var res = await _http.DeleteAsync($"Task/{id}");
        return res.IsSuccessStatusCode;
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