namespace BrainWave.App.Models;


public class CollaborationDto
{
    public int CollaborationID { get; set; }
    public int TaskID { get; set; }
    public string? Collaboration_Title { get; set; }
    public string? Collaboration_Description { get; set; }
}


public class UserCollaborationDto
{
    public int UserID { get; set; }
    public int CollaborationID { get; set; }
    public string? Collaboration_Role { get; set; }
}


public record CollaborationCreateDto(int TaskID, string? Collaboration_Title, string? Collaboration_Description);
public record AddUserToCollabDto(int UserID, int CollaborationID, string? Collaboration_Role);