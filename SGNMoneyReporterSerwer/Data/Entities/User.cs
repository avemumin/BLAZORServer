using System;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class User
    {
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserPassword { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LastEditDate { get; set; }
        public byte Role { get; set; }
    }
}
