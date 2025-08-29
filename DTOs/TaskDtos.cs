using BrainWave.API.Entities;

namespace BrainWave.Api.DTOs
{
    public class TaskDtos
    {
        public int TaskID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime? Due_Date { get; set; }
        public TaskStatus Task_Status { get; set; } = TaskStatus.Pending;
        public TaskPriority Priority_Level { get; set; } = TaskPriority.Medium;
        public DateTime Created_Date { get; set; }
        public DateTime? Updated_Date { get; set; }
        public bool IsShared { get; set; }
        public string? ShareToken { get; set; }
    }

    public class TaskFilterDto
    {
        public TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public string? SortBy { get; set; } = "Created_Date";
        public bool SortDescending { get; set; } = true;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
