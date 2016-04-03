using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigData
{
    public class UserInfo
    {
        public string User { get; set; }
        public string Pass { get; set; }
        public string Host { get; set; }

        public UserInfo()
        {
            this.User = string.Empty;
            this.Pass = string.Empty;
            this.Host = string.Empty;
        }
    }
}
