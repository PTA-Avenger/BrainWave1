namespace BrainWave.Api.DTOs
{
    public class UserDtos
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
        public bool IsActive { get; set; } = true;
    }

    public class UpdateUserDto
    {
        public string F_Name { get; set; } = "";
        public string L_Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public string? Profile_Picture { get; set; }
    }

    public class UserFilterDto
    {
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; } = "Created_Date";
        public bool SortDescending { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
