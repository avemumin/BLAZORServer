using System;
using System.Collections.Generic;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class CountDetail
    {
        public long IdCountDetail { get; set; }
        public long IdCountResult { get; set; }
        public int Id { get; set; }
        public short IdCurrency { get; set; }
        public short IdCurrencyFaceValue { get; set; }
        public string BanknoteSn { get; set; }

        public virtual CountResult IdCountResultNavigation { get; set; }
        public virtual CurrencyFaceValue IdCurrencyFaceValueNavigation { get; set; }
        public virtual Currency IdCurrencyNavigation { get; set; }
    }
}
