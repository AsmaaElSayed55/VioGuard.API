using Domain.Entities.SystemModule.ModelsModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SystemModule
{
    public class SystemRoot : BaseEntity<int>
    {
        public ICollection<History> Histories { get; set; }
        public ICollection<Model> Models { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}
