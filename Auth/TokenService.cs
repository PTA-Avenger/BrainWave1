using BrainWave.Api.Entities;
using BrainWave.API.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace BrainWave.API.Auth;


public class TokenService
{
    private readonly JwtSettings _settings;
    public TokenService(IOptions<JwtSettings> settings) => _settings = settings.Value;


    public string CreateToken(User user)
    {
        var claims = new List<Claim>
{
new(ClaimTypes.NameIdentifier, user.UserID.ToString()),
new(ClaimTypes.Name, $"{user.F_Name} {user.L_Name}"),
new(ClaimTypes.Email, user.Email),
new(ClaimTypes.Role, user.Role ?? "User")
};


        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
        issuer: _settings.Issuer,
        audience: _settings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_settings.ExpiresMinutes),
        signingCredentials: creds);


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}