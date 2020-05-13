using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public class SerialNumbersDuplicatesSP
    {
        public int IdMachine { get; set; }
        public string SN { get; set; }
        public int Counts { get; set; }
        public string BanknoteSN { get; set; }
        public short IdCurrencyFaceValue { get; set; }
        public short IdCurrency { get; set; }
        public string Symbol { get; set; }
        public decimal FaceValue { get; set; }
    }
}
