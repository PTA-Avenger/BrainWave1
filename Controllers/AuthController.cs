using BrainWave.Api.DTOs;
using BrainWave.API.DTOs;
using BrainWave.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(UserRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDtos>> Register([FromBody] RegisterDtos dto)
        {
            if (await _repo.UserExistsAsync(dto.Email))
                return BadRequest("Email already exists.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                F_Name = dto.F_Name,
                L_Name = dto.L_Name,
                Email = dto.Email,
                Password_Hash = hashedPassword,
                Role = "User"
            };

            await _repo.AddUserAsync(user);

            return new UserDtos
            {
                UserID = user.UserID,
                F_Name = user.F_Name,
                L_Name = user.L_Name,
                Email = user.Email,
                Role = user.Role
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDtos dto)
        {
            var user = await _repo.GetUserByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid credentials.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password_Hash))
                return Unauthorized("Invalid credentials.");

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
