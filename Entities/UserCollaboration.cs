using BrainWave.API.Entities;

namespace BrainWave.Api.Entities
{
    public class UserCollaboration
    {
        public int UserID { get; set; }
        public int CollaborationID { get; set; }
        public string? Collaboration_Role { get; set; }

        // Navigation
        public User? User { get; set; }
        public Collaboration? Collaboration { get; set; }
    }
}
