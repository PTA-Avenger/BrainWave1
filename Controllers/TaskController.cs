using BrainWave.Api.DTOs;
using BrainWave.API.DTOs;
using BrainWave.API.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using Task = BrainWave.API.Entities.Tasks;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TasksRepository _repo;
        public TaskController(TasksRepository repo) => _repo = repo;

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<TaskDtos>>> GetTasks(int userId)
        {
            var tasks = await _repo.GetTasksByUserIdAsync(userId);

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDtos>> CreateTask([FromBody] TaskDtos dto)
        {
            var task = new Task
            {
                UserID = 1, // TODO: replace with UserID from JWT
                Title = dto.Title,
                Description = dto.Description,
                Due_Date = dto.Due_Date,
                Task_Status = dto.Task_Status,
                Priority_Level = dto.Priority_Level
            };

            await _repo.AddTaskAsync(task);

            dto.TaskID = task.TaskID;
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDtos dto)
        {
            var task = await _repo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Due_Date = dto.Due_Date;
            task.Task_Status = dto.Task_Status;
            task.Priority_Level = dto.Priority_Level;

            await _repo.UpdateTaskAsync(task);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _repo.GetTaskByIdAsync(id);
            if (task == null) return NotFound();

            await _repo.DeleteTaskAsync(task);
            return NoContent();
        }
    }
}
