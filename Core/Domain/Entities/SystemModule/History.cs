using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SystemModule
{
    public class History : BaseEntity<int>
    {
        public int SystemId { get; set; }
        public SystemRoot System { get; set; }
    }
}
