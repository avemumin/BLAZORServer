using System.Collections.Generic;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class Currency
    {
        public Currency()
        {
            CountDetail = new HashSet<CountDetail>();
        }

        public short IdCurrency { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }

        public virtual ICollection<CountDetail> CountDetail { get; set; }
    }
}
