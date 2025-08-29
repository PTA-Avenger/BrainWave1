using BrainWave.Api.DTOs;
using BrainWave.Api.Entities;
using BrainWave.API.DTOs;
using BrainWave.API.Entities;
using BrainWave.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BrainWave.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReminderController : ControllerBase
    {
        private readonly ReminderRepository _repo;
        private readonly TasksRepository _taskRepo;
        private readonly UserRepository _userRepo;
        private readonly IEmailService _emailService;

        public ReminderController(ReminderRepository repo, TasksRepository taskRepo, UserRepository userRepo, IEmailService emailService)
        {
            _repo = repo;
            _taskRepo = taskRepo;
            _userRepo = userRepo;
            _emailService = emailService;
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<IEnumerable<ReminderDtos>>> GetReminders(int taskId)
        {
            var reminders = await _repo.GetRemindersByTaskIdAsync(taskId);
            return Ok(reminders.Select(r => new ReminderDtos
            {
                ReminderID = r.ReminderID,
                TaskID = r.TaskID,
                Reminder_Type = r.Reminder_Type,
                Notify_Time = r.Notify_Time
            }));
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ReminderDtos>>> GetUserReminders(int userId)
        {
            var reminders = await _repo.GetRemindersByUserIdAsync(userId);
            return Ok(reminders.Select(r => new ReminderDtos
            {
                ReminderID = r.ReminderID,
                TaskID = r.TaskID,
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
                Notify_Time = dto.Notify_Time,
                Created_Date = DateTime.UtcNow,
                IsActive = true,
                IsEmailSent = false
            };

            await _repo.AddReminderAsync(reminder);

            dto.ReminderID = reminder.ReminderID;
            return Ok(dto);
        }

        [HttpPost("send-due-reminders")]
        public async Task<ActionResult> SendDueReminders()
        {
            var dueReminders = await _repo.GetDueRemindersAsync();
            var emailsSent = 0;

            foreach (var reminder in dueReminders)
            {
                try
                {
                    var task = await _taskRepo.GetTaskByIdAsync(reminder.TaskID);
                    if (task?.User != null)
                    {
                        var subject = $"Reminder: {task.Title}";
                        var body = $@"
                        <html>
                        <body>
                            <h2>Task Reminder</h2>
                            <p>This is a reminder for your task: <strong>{task.Title}</strong></p>
                            <p><strong>Description:</strong> {task.Description}</p>
                            <p><strong>Due Date:</strong> {task.Due_Date?.ToString("MM/dd/yyyy HH:mm")}</p>
                            <p><strong>Priority:</strong> {task.Priority_Level}</p>
                            <p><strong>Status:</strong> {task.Task_Status}</p>
                            <br>
                            <p>Best regards,<br>BrainWave Team</p>
                        </body>
                        </html>";

                        await _emailService.SendReminderEmailAsync(task.User.Email, subject, body);
                        
                        reminder.IsEmailSent = true;
                        await _repo.UpdateReminderAsync(reminder);
                        emailsSent++;
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue with other reminders
                    Console.WriteLine($"Failed to send reminder {reminder.ReminderID}: {ex.Message}");
                }
            }

            return Ok(new { EmailsSent = emailsSent, Message = $"Sent {emailsSent} reminder emails" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(int id, [FromBody] ReminderDtos dto)
        {
            var reminder = await _repo.GetReminderByIdAsync(id);
            if (reminder == null) return NotFound();

            reminder.Reminder_Type = dto.Reminder_Type;
            reminder.Notify_Time = dto.Notify_Time;

            await _repo.UpdateReminderAsync(reminder);
            return NoContent();
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
