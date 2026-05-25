using AutoMapper;
using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
using Shared.Dtos;
namespace Services.MappingProfiles
{
    public class ContentProfile : Profile
    {
        public ContentProfile()
        {

            CreateMap<UploadTextDto, TextContent>()
                .ForMember(dest => dest.URL, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.textContext, opt => opt.MapFrom(src => src.TextContext))
                .ForMember(dest => dest.textContext, opt => opt.MapFrom(_ => "Text"))   ////////
                .ForMember(dest => dest.DetectionDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserEmail, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ViolentResult, opt => opt.Ignore())
                .ForMember(dest => dest.ViolentWords, opt => opt.Ignore());

            CreateMap<UploadVideoDto, VideoContent>()
                .ForMember(dest => dest.URL, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.ViolentPercent, opt => opt.MapFrom(src => src.ViolentPercent))
                .ForMember(dest => dest.ViolentPercent, opt => opt.MapFrom(_ => "Video")) ////////
                .ForMember(dest => dest.DetectionDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserEmail, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());


            CreateMap<History, HistoryResultDto>()
                .ForCtorParam(nameof(HistoryResultDto.Id), opt => opt.MapFrom(src => src.Id))
                .ForCtorParam(nameof(HistoryResultDto.ContentUrl), opt => opt.MapFrom(src => src.ContentUrl))
                .ForCtorParam(nameof(HistoryResultDto.ContentType), opt => opt.MapFrom(src => src.ContentType))
                .ForCtorParam(nameof(HistoryResultDto.ActionDate), opt => opt.MapFrom(src => src.ActionDate))
                .ForCtorParam(nameof(HistoryResultDto.AttachedUserEmail), opt => opt.MapFrom(src => src.AttachedUserEmail));

            CreateMap<CreateHistoryDto, History>()
                .ForMember(dest => dest.AttachedUserEmail, opt => opt.MapFrom(src => src.UserEmail))
                .ForMember(dest => dest.ActionDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SystemId, opt => opt.Ignore())
                .ForMember(dest => dest.System, opt => opt.Ignore());
        }
    }
}