using Domain.Entities.UserModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SystemModule
{
    public class Report : BaseEntity<int>
    {
        public int NumOfVideo { get; set; }
        public int NumOfText { get; set; }
        public int ViolentText { get; set; }
        public int ViolentVideo { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
