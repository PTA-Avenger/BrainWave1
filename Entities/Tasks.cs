using BrainWave.Api.Entities;
using System;
using System.Collections.Generic;

namespace BrainWave.API.Entities
{
    public class Tasks
    {
        public int TaskID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime? Due_Date { get; set; }
        public TaskStatus Task_Status { get; set; } = TaskStatus.Pending;
        public TaskPriority Priority_Level { get; set; } = TaskPriority.Medium;
        public DateTime Created_Date { get; set; } = DateTime.UtcNow;
        public DateTime? Updated_Date { get; set; }
        public bool IsShared { get; set; } = false;
        public string? ShareToken { get; set; }

        // Navigation properties
        public User? User { get; set; }

        // Optional single collaboration
        public Collaboration? PrimaryCollaboration { get; set; }

        // Add this property to the Task class
        public ICollection<Collaboration> Collaborations { get; set; }

        public ICollection<Reminder> Reminders { get; set; } = new HashSet<Reminder>();
        public ICollection<Export> Exports { get; set; } = new HashSet<Export>();
        // In the Task class constructor, initialize the collection (optional but recommended)
        public Tasks()
        {
            Collaborations = new HashSet<Collaboration>();
        }
    }
}
