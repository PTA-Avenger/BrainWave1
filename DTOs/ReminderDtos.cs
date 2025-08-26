namespace BrainWave.Api.DTOs
{
    public class ReminderDtos
    {
        public int ReminderID { get; set; }
        public int TaskID { get; set; } // Added property
        public string? Reminder_Type { get; set; }
        public DateTime? Notify_Time { get; set; }
    }
}
