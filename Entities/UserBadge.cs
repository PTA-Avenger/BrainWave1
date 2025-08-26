using BrainWave.API.Entities;

namespace BrainWave.Api.Entities
{
    public class UserBadge
    {
        public int UserID { get; set; }
        public int BadgeID { get; set; }
        public DateTime? Date_Earned { get; set; }

        // Navigation
        public User? User { get; set; }
        public Badge? Badge { get; set; }
    }
}
