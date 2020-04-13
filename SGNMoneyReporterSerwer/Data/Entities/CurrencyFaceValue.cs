using System.Collections.Generic;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class CurrencyFaceValue
    {
        public CurrencyFaceValue()
        {
            CountDetail = new HashSet<CountDetail>();
            CountSummary = new HashSet<CountSummary>();
        }

        public short IdCurrencyFaceValue { get; set; }
        public decimal FaceValue { get; set; }

        public virtual ICollection<CountDetail> CountDetail { get; set; }
        public virtual ICollection<CountSummary> CountSummary { get; set; }
    }
}
