namespace BrainWave.App.Models;


public class TaskDto
{
    public int TaskID { get; set; }
    public int UserID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Task_Status { get; set; }
    public string? Priority_Level { get; set; }
    public DateOnly? Due_Date { get; set; }
}


public record TaskCreateDto(int UserID, string Title, string? Description, DateOnly? Due_Date, string? Task_Status, string? Priority_Level);
public record TaskUpdateDto(string? Title, string? Description, DateOnly? Due_Date, string? Task_Status, string? Priority_Level);