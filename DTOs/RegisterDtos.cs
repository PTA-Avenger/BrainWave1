namespace BrainWave.API.DTOs
{
    public class RegisterDtos
    {
        public string F_Name { get; set; } = "";
        public string L_Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string? Role { get; set; }
    }
}
