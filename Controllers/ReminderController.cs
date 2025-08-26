using BrainWave.Api.DTOs;
using BrainWave.Api.Entities;
using BrainWave.API.DTOs;
using BrainWave.API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderController : ControllerBase
    {
        private readonly ReminderRepository _repo;
        public ReminderController(ReminderRepository repo) => _repo = repo;

        [HttpGet("{taskId}")]
        public async Task<ActionResult<IEnumerable<ReminderDtos>>> GetReminders(int taskId)
        {
            var reminders = await _repo.GetRemindersByTaskIdAsync(taskId);
            return Ok(reminders.Select(r => new ReminderDtos
            {
                ReminderID = r.ReminderID,
                Reminder_Type = r.Reminder_Type,
                Notify_Time = r.Notify_Time
            }));
        }

        [HttpPost]
        public async Task<ActionResult<ReminderDtos>> CreateReminder([FromBody] ReminderDtos dto)
        {
            var reminder = new Reminder
            {
                TaskID = dto.TaskID,
                Reminder_Type = dto.Reminder_Type,
                Notify_Time = dto.Notify_Time
            };

            await _repo.AddReminderAsync(reminder);

            dto.ReminderID = reminder.ReminderID;
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            var reminder = await _repo.GetReminderByIdAsync(id);
            if (reminder == null) return NotFound();

            await _repo.DeleteReminderAsync(id);
            return NoContent();
        }
    }
}
