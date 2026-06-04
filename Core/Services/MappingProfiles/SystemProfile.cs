using AutoMapper;
using Domain.Entities.SystemModule;
using Domain.Entities.SystemModule.ModelsModule;
using Shared.Dtos.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfiles
{
    public class SystemProfile : Profile
    {
        public SystemProfile()
        {
            CreateMap<SystemRoot, SystemRootDto>();
            CreateMap<AIModel, AIModelDto>();
            CreateMap<HistoryRecord, HistoryRecordDto>();
        }
    }
}
