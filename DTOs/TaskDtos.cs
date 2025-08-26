namespace BrainWave.Api.DTOs
{
    public class TaskDtos
    {
        public int TaskID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Due_Date { get; set; }
        public string? Task_Status { get; set; }
        public string? Priority_Level { get; set; }
    }
}
