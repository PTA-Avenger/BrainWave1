namespace BrainWave.App.Models;


public class BadgeDto
{
    public int BadgeID { get; set; }
    public string? Badge_Type { get; set; }
    public string? Badge_Description { get; set; }
}


public class UserBadgeDto
{
    public int UserID { get; set; }
    public int BadgeID { get; set; }
    public DateOnly? Date_Earned { get; set; }
    public BadgeDto? Badge { get; set; }
}


public record BadgeCreateDto(string? Badge_Type, string? Badge_Description);
public record AssignBadgeDto(int UserID, int BadgeID, DateOnly? Date_Earned);