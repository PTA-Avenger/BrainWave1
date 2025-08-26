namespace BrainWave.API.ViewModels
{
    public class TaskViewModel
    {
        public int TaskID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string TaskStatus { get; set; }
        public string PriorityLevel { get; set; }

        // Navigation helpers
        public string AssignedUser { get; set; }   // F_Name + L_Name
    }
}
