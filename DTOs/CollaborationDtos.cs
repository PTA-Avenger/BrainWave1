namespace BrainWave.Api.DTOs
{
    public class CollaborationDtos
    {
        public int CollaborationID { get; set; }
        public int TaskID { get; set; }
        public string? Collaboration_Title { get; set; }
        public string? Collaboration_Description { get; set; }
        public string? InvitePin { get; set; }
        public DateTime Created_Date { get; set; }
        public DateTime? Pin_Expiry { get; set; }
        public bool IsActive { get; set; }
        public List<string> CollaboratorNames { get; set; } = new List<string>();
    }

    public class JoinCollaborationDto
    {
        public string InvitePin { get; set; } = "";
        public int UserID { get; set; }
    }

    public class CreateCollaborationDto
    {
        public int TaskID { get; set; }
        public string? Collaboration_Title { get; set; }
        public string? Collaboration_Description { get; set; }
    }
}
