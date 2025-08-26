using BrainWave.Api.DTOs;
using BrainWave.API.DTOs;
using BrainWave.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                Role = u.Role
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
                Role = u.Role
            };
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
                Role = dto.Role
            };

            await _repo.AddUserAsync(user);

            dto.UserID = user.UserID;
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDtos dto)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.F_Name = dto.F_Name;
            user.L_Name = dto.L_Name;
            user.Role = dto.Role;

            await _repo.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _repo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            await _repo.DeleteUserAsync(user.UserID);
            return NoContent();
        }
    }
}
