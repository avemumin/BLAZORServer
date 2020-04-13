using System;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class FileHistory
    {
        public long IdFileHistory { get; set; }
        public string FileName { get; set; }
        public bool IsProceededSuccess { get; set; }
        public string ErrorDescription { get; set; }
        public DateTime ProcessDate { get; set; }
        public long? IdCountResult { get; set; }

        public virtual CountResult IdCountResultNavigation { get; set; }
    }
}
