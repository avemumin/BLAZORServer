using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGNMoneyReporterSerwer.Data.Entities
{
    public class UserWithToken : User
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public UserWithToken()
        {
            
        }
        public UserWithToken(User user)
        {
            this.IdUser = user.IdUser;
            this.UserName = user.UserName;
            this.UserLastName = user.UserLastName;
            this.UserEmailAddress = user.UserEmailAddress;
            this.UserPassword = user.UserPassword;
            this.IsActive = user.IsActive;
            this.LastEditDate = user.LastEditDate;
            this.Role = user.Role;
        }
    }
}


