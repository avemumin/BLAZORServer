using System;
using System.Collections.Generic;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class Machine
    {
        public Machine()
        {
            CountResult = new HashSet<CountResult>();
        }

        public int IdMachine { get; set; }
        public string Model { get; set; }
        public string Sn { get; set; }
        public string SoftwareVersion { get; set; }

        public virtual ICollection<CountResult> CountResult { get; set; }
    }
}
