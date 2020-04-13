using System.Collections.Generic;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class Cashier
    {
        public Cashier()
        {
            CountResult = new HashSet<CountResult>();
        }

        public int IdCashier { get; set; }
        public string CashierName { get; set; }

        public virtual ICollection<CountResult> CountResult { get; set; }
    }
}
