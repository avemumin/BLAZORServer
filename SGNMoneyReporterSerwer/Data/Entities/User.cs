using System;
using System.Collections.Generic;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public partial class User
    {
        public User()
        {
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserPassword { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastEditDate { get; set; }
        public byte RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
