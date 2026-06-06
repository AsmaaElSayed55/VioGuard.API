using AutoMapper;
using Domain.Entities.ContentsMudule;
using Shared.Dtos.Content;

namespace Services.MappingProfiles
{
    public class ContentProfile : Profile
    {
        public ContentProfile()
        {
            CreateMap<Content, ContentDto>()
                .ForMember(dest => dest.URL, opt => opt.MapFrom(src => src.Id));

            CreateMap<TextContent, TextContentDto>()
                .ForMember(dest => dest.URL, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.textContext, opt => opt.MapFrom(src => src.textContext))
                .ForMember(dest => dest.ViolentWords, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.ViolentWords)
                        ? new List<string>()
                        : src.ViolentWords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()));

            CreateMap<VideoContent, VideoContentDto>()
                .ForMember(dest => dest.URL, opt => opt.MapFrom(src => src.Id));

            CreateMap<CreateTextContentDto, TextContent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.URL))
                .ForMember(dest => dest.textContext, opt => opt.MapFrom(src => src.TextContext))
                .ForMember(dest => dest.ViolentWords, opt => opt.MapFrom(src => string.Join(", ", src.ViolentWords)))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(_ => "Text"));

            CreateMap<CreateVideoContentDto, VideoContent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.URL))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(_ => "Video"));
        }
    }
}
