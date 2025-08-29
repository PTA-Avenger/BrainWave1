using BrainWave.Api.DTOs;
using BrainWave.API.DTOs;
using BrainWave.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _repo;
        public UserController(UserRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDtos>>> GetUsers()
        {
            var users = await _repo.GetUsersAsync();
            return Ok(users.Select(u => new UserDtos
            {
                UserID = u.UserID,
                F_Name = u.F_Name,
                L_Name = u.L_Name,
                Email = u.Email,
                Role = u.Role,
                Profile_Picture = u.Profile_Picture,
                Phone = u.Phone,
                Bio = u.Bio,
                Created_Date = u.Created_Date,
                Updated_Date = u.Updated_Date,
                IsActive = u.IsActive
            }));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDtos>> GetUser(int id)
        {
            var u = await _repo.GetUserByIdAsync(id);
            if (u == null) return NotFound();

            return new UserDtos
            {
                UserID = u.UserID,
                F_Name = u.F_Name,
                L_Name = u.L_Name,
                Email = u.Email,
                Role = u.Role,
                Profile_Picture = u.Profile_Picture,
                Phone = u.Phone,
                Bio = u.Bio,
                Created_Date = u.Created_Date,
                Updated_Date = u.Updated_Date,
                IsActive = u.IsActive
            };
        }

        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.F_Name = dto.F_Name;
            user.L_Name = dto.L_Name;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.Bio = dto.Bio;
            user.Profile_Picture = dto.Profile_Picture;
            user.Updated_Date = DateTime.UtcNow;

            await _repo.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateAccount(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.IsActive = false;
            user.Updated_Date = DateTime.UtcNow;

            await _repo.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            // In a real application, you might want to anonymize data instead of hard delete
            await _repo.DeleteUserAsync(user.UserID);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<UserDtos>> CreateUser([FromBody] UserDtos dto)
        {
            var user = new User
            {
                F_Name = dto.F_Name,
                L_Name = dto.L_Name,
                Email = dto.Email,
                Password_Hash = "hashed_password", // replace with bcrypt
                Role = dto.Role,
                Phone = dto.Phone,
                Bio = dto.Bio,
                Profile_Picture = dto.Profile_Picture,
                Created_Date = DateTime.UtcNow,
                IsActive = true
            };

            await _repo.AddUserAsync(user);

            dto.UserID = user.UserID;
            dto.Created_Date = user.Created_Date;
            return Ok(dto);
        }
    }
}
