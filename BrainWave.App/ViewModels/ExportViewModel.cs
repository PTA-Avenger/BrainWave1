using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainWave.App.ViewModels
{
    internal class ExportViewModel
    {
        public int ExportID { get; set; }
        public string ExportFormat { get; set; }
        public DateTime DateRequested { get; set; }

        // Related info
        public string TaskTitle { get; set; }
        public string RequestedBy { get; set; }
    }
}
