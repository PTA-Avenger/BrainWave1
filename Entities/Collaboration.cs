using BrainWave.API.Entities;

namespace BrainWave.Api.Entities
{
    public class Collaboration
    {
        public int CollaborationID { get; set; }
        public int TaskID { get; set; }
        public string? Collaboration_Title { get; set; }
        public string? Collaboration_Description { get; set; }
        public string? InvitePin { get; set; }
        public DateTime Created_Date { get; set; } = DateTime.UtcNow;
        public DateTime? Pin_Expiry { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public Tasks? Task { get; set; }
        public ICollection<UserCollaboration> UserCollaborations { get; set; } = new List<UserCollaboration>();
    }
}
