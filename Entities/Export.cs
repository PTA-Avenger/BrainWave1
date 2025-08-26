using BrainWave.API.Entities;
using Tasks = BrainWave.API.Entities.Tasks;

namespace BrainWave.Api.Entities
{
    public class Export
    {
        public int ExportID { get; set; }
        public int UserID { get; set; }
        public int TaskID { get; set; }
        public string? Export_Format { get; set; }
        public DateTime? Date_Requested { get; set; }

        // Navigation
        public User? User { get; set; }
        public Tasks? Task { get; set; }
    }
}
