using BrainWave.Api.DTOs;
using BrainWave.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BrainWave.API.Auth;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserRepository _userRepo;
        private readonly TasksRepository _taskRepo;
        private readonly TokenService _tokenService;
        
        // Hard-coded admin credentials (as requested)
        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin123!";

        public AdminController(UserRepository userRepo, TasksRepository taskRepo, TokenService tokenService)
        {
            _userRepo = userRepo;
            _taskRepo = taskRepo;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> AdminLogin([FromBody] AdminLoginDto dto)
        {
            if (dto.Username != AdminUsername || dto.Password != AdminPassword)
            {
                return Unauthorized("Invalid admin credentials");
            }

            // Create a special admin user object for token generation
            var adminUser = new User
            {
                UserID = -1,
                F_Name = "Admin",
                L_Name = "User",
                Email = "admin@brainwave.com",
                Role = "Admin"
            };

            var token = _tokenService.GenerateToken(adminUser);
            return Ok(new { Token = token, Role = "Admin" });
        }

        [HttpGet("users")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AdminUserDto>>> GetAllUsers([FromQuery] UserFilterDto? filter = null)
        {
            // Verify admin role (in a real app, you'd check the JWT claims)
            var users = await _userRepo.GetFilteredUsersAsync(filter);
            var adminUsers = users.Select(u => new AdminUserDto
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
                IsActive = u.IsActive,
                TaskCount = u.Tasks?.Count ?? 0
            });

            return Ok(adminUsers);
        }

        [HttpGet("tasks")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AdminTaskDto>>> GetAllTasks([FromQuery] TaskFilterDto? filter = null)
        {
            var tasks = await _taskRepo.GetAllFilteredTasksAsync(filter);
            var adminTasks = tasks.Select(t => new AdminTaskDto
            {
                TaskID = t.TaskID,
                UserID = t.UserID,
                UserName = t.User != null ? $"{t.User.F_Name} {t.User.L_Name}" : "Unknown",
                Title = t.Title,
                Description = t.Description,
                Due_Date = t.Due_Date,
                Task_Status = t.Task_Status,
                Priority_Level = t.Priority_Level,
                Created_Date = t.Created_Date,
                Updated_Date = t.Updated_Date,
                IsShared = t.IsShared
            });

            return Ok(adminTasks);
        }

        [HttpPut("users/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDtos dto)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.F_Name = dto.F_Name;
            user.L_Name = dto.L_Name;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.Profile_Picture = dto.Profile_Picture;
            user.Phone = dto.Phone;
            user.Bio = dto.Bio;
            user.IsActive = dto.IsActive;
            user.Updated_Date = DateTime.UtcNow;

            await _userRepo.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("users/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            await _userRepo.DeleteUserAsync(user);
            return NoContent();
        }

        [HttpPut("tasks/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDtos dto)
        {
            var task = await _taskRepo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Due_Date = dto.Due_Date;
            task.Task_Status = dto.Task_Status;
            task.Priority_Level = dto.Priority_Level;
            task.Updated_Date = DateTime.UtcNow;

            await _taskRepo.UpdateTaskAsync(task);
            return NoContent();
        }

        [HttpDelete("tasks/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskRepo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            await _taskRepo.DeleteTaskAsync(task);
            return NoContent();
        }
    }
}