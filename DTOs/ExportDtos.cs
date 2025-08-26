namespace BrainWave.API.DTOs
{
    public class ExportDtos
    {
        public int ExportID { get; set; }
        public int UserID { get; set; } // Added property
        public int TaskID { get; set; } // Added property
        public string? Export_Format { get; set; }
        public DateTime? Date_Requested { get; set; }
    }
}
