using System.Collections.Generic;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class Quality
    {
        public Quality()
        {
            CountSummary = new HashSet<CountSummary>();
        }

        public short IdQuality { get; set; }
        public string QualityValue { get; set; }

        public virtual ICollection<CountSummary> CountSummary { get; set; }
    }
}
