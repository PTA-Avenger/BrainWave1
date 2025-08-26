using BrainWave.App.Models;


namespace BrainWave.App.Services;


public class AuthService
{
    private const string TokenKey = "bw_token";
    private const string UserIdKey = "bw_userid";


    public string? Token { get; private set; }
    public int? CurrentUserId { get; private set; }


    public async Task SaveAuthAsync(AuthResponse auth)
    {
        Token = auth.Token;
        CurrentUserId = auth.UserID;
        await SecureStorage.SetAsync(TokenKey, auth.Token);
        await SecureStorage.SetAsync(UserIdKey, auth.UserID.ToString());
    }


    public async Task LoadAsync()
    {
        Token = await SecureStorage.GetAsync(TokenKey);
        var idStr = await SecureStorage.GetAsync(UserIdKey);
        if (int.TryParse(idStr, out var id)) CurrentUserId = id;
    }


    public async Task LogoutAsync()
    {
        Token = null;
        CurrentUserId = null;
        SecureStorage.Remove(TokenKey);
        SecureStorage.Remove(UserIdKey);
        await Task.CompletedTask;
    }
}