using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public class QualitySP
    {
        public short IdCurrencyFaceValue { get; set; }
        public decimal FaceValue { get; set; }
        public long CountedCount { get; set; }
        public long Count { get; set; }
        public string QualityValue { get; set; }
        public string Symbol { get; set; }
        public string ModeValue { get; set; }
    }
}
