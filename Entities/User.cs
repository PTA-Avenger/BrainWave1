using BrainWave.Api.Entities;

namespace BrainWave.API.Entities
{
    public class User
    {
        public int UserID { get; set; }
        public string F_Name { get; set; } = "";
        public string L_Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password_Hash { get; set; } = "";
        public string? Role { get; set; }
        public string? Profile_Picture { get; set; }

        // Navigation
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
        public ICollection<UserCollaboration> UserCollaborations { get; set; } = new List<UserCollaboration>();
    
    }
}