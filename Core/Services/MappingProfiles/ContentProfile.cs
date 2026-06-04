using AutoMapper;
using Domain.Entities.ContentsMudule;
using Domain.Entities.UserModule;
using Shared.Dtos.AI_Models;
using Shared.Dtos.Content;
using System;

namespace Services.MappingProfiles
{
    public class ContentMappingProfile : Profile
    {
        public class ContentProfile : Profile
        {
            public ContentProfile()
            {
                // Base Mapping
                CreateMap<Content, ContentDto>()
                    .ForMember(dest => dest.URL, opt => opt.MapFrom(src => src.Id))
                    .Include<TextContent, TextContentDto>()
                    .Include<VideoContent, VideoContentDto>();

                // Subclass Mappings
                CreateMap<TextContent, TextContentDto>();
                CreateMap<VideoContent, VideoContentDto>();

                // Request mappings
                CreateMap<CreateTextContentDto, TextContent>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.URL));

                CreateMap<CreateVideoContentDto, VideoContent>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.URL));
            }
        }
    }
}