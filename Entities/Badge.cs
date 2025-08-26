namespace BrainWave.Api.Entities
{
    public class Badge
    {
        public int BadgeID { get; set; }
        public string? Badge_Type { get; set; }
        public string? Badge_Description { get; set; }

        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}
