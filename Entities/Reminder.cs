using BrainWave.API.Entities;

namespace BrainWave.Api.Entities
{
    public class Reminder
    {
        public int ReminderID { get; set; }
        public int TaskID { get; set; }
        public string? Reminder_Type { get; set; }
        public DateTime? Notify_Time { get; set; }
        public bool IsEmailSent { get; set; } = false;
        public DateTime Created_Date { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation
        public Tasks? Task { get; set; }
    }
}
