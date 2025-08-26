namespace BrainWave.API.ViewModels
{
    public class UserViewModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; }   // Combine F_Name + L_Name
        public string Email { get; set; }
        public string Role { get; set; }
        public string ProfilePicture { get; set; }
    }
}
