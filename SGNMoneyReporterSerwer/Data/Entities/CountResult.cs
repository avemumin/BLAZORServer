using System;
using System.Collections.Generic;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class CountResult
    {
        public CountResult()
        {
            CountDetail = new HashSet<CountDetail>();
            CountSummary = new HashSet<CountSummary>();
            FileHistory = new HashSet<FileHistory>();
        }

        public long IdCountResult { get; set; }
        public int IdMachine { get; set; }
        public string MachineSoftVer { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SavedDate { get; set; }
        public short IdMode { get; set; }
        public int CountedCount { get; set; }
        public int RejectedCount { get; set; }
        public string BagNo { get; set; }
        public int IdCashier { get; set; }

        public virtual Cashier IdCashierNavigation { get; set; }
        public virtual Machine IdMachineNavigation { get; set; }
        public virtual Mode IdModeNavigation { get; set; }
        public virtual ICollection<CountDetail> CountDetail { get; set; }
        public virtual ICollection<CountSummary> CountSummary { get; set; }
        public virtual ICollection<FileHistory> FileHistory { get; set; }
    }
}
