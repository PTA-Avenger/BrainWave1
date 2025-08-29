using BrainWave.API.Entities;

namespace BrainWave.Api.DTOs
{
    public class AdminLoginDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class AdminUserDto
    {
        public int UserID { get; set; }
        public string F_Name { get; set; } = "";
        public string L_Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Role { get; set; }
        public string? Profile_Picture { get; set; }
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public DateTime Created_Date { get; set; }
        public DateTime? Updated_Date { get; set; }
        public bool IsActive { get; set; }
        public int TaskCount { get; set; }
    }

    public class AdminTaskDto
    {
        public int TaskID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime? Due_Date { get; set; }
        public TaskStatus Task_Status { get; set; }
        public TaskPriority Priority_Level { get; set; }
        public DateTime Created_Date { get; set; }
        public DateTime? Updated_Date { get; set; }
        public bool IsShared { get; set; }
    }
}