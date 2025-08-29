namespace BrainWave.App.Models;


public record RegisterRequest(string F_Name, string L_Name, string Email, string Password, string? Role, string? Profile_Picture);
public record LoginRequest(string Email, string Password);


public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public int UserID { get; set; }
    public string F_Name { get; set; } = string.Empty;
    public string L_Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; }
}