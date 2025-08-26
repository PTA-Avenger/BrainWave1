using BrainWave.API.Entities;

namespace BrainWave.Api.Entities
{
    public class Collaboration
    {
        public int CollaborationID { get; set; }
        public int TaskID { get; set; }
        public string? Collaboration_Title { get; set; }
        public string? Collaboration_Description { get; set; }

        // Navigation
        public Tasks? Task { get; set; }
        public ICollection<UserCollaboration> UserCollaborations { get; set; } = new List<UserCollaboration>();
    }
}
