using AutoMapper;
using Domain.Entities.ContentsMudule;
using Domain.Entities.UserModule;
using Shared.Dtos;
using System;

namespace Services.MappingProfiles
{
    public class ContentProfile : Profile
    {
        public ContentProfile()
        {
            // Text Content Map
            CreateMap<UploadTextDto, TextContent>()
                .ForMember(dest => dest.URL, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.textContext, opt => opt.MapFrom(src => src.TextContext))
                .ForMember(dest => dest.DetectionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserEmail, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ViolentResult, opt => opt.Ignore())
                .ForMember(dest => dest.ViolentWords, opt => opt.Ignore());

            // Video Content Map
            CreateMap<UploadVideoDto, VideoContent>()
                .ForMember(dest => dest.URL, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.ViolentPercent, opt => opt.MapFrom(src => src.ViolentPercent))
                .ForMember(dest => dest.DetectionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserEmail, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // History Maps (Keep your existing history mapping code here unchanged)
        }
    }
}