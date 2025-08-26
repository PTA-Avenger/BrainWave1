namespace BrainWave.App.Models;


public class ReminderDto
{
    public int ReminderID { get; set; }
    public int TaskID { get; set; }
    public string? Reminder_Type { get; set; }
    public DateTime? Notify_Time { get; set; }
}


public record ReminderCreateDto(int TaskID, string? Reminder_Type, DateTime? Notify_Time);