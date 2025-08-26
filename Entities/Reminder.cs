using BrainWave.API.Entities;

namespace BrainWave.Api.Entities
{
    public class Reminder
    {
        public int ReminderID { get; set; }
        public int TaskID { get; set; }
        public string? Reminder_Type { get; set; }
        public DateTime? Notify_Time { get; set; }

        // Navigation
        public Tasks? Task { get; set; }
    }
}
