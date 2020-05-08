using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGNMoneyReporterSerwer.Models
{
    public class FileHistoryModel
    {
        public long IdFileHistory { get; set; }
        public string FileName { get; set; }
        public bool IsProceededSuccess { get; set; }
        public string ErrorDescription { get; set; }
        public DateTime ProcessDate { get; set; }
        public long? IdCountResult { get; set; }
    }
}
