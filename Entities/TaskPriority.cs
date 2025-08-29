namespace BrainWave.API.Entities
{
    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }
}