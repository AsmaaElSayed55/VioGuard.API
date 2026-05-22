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
        public string SystemName { get; set; } = "VioGuard Engine";

        // Navigation Properties
        public ICollection<History> Histories { get; set; } = new List<History>();
        public ICollection<Model> Models { get; set; } = new List<Model>();
    }
}
