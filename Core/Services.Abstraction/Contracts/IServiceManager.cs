using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstraction.Contracts
{
    public interface IServiceManager
    {
        public IHistoryService HistoryService { get; }

    }
}
