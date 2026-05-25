using AutoMapper;
using Domain.Entities.UserModule;
using Shared.Dtos.User;

namespace Services.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                // 1. Map domain entity 'UserInternalId' property to DTO 'Id' field
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserInternalId))

                // 2. Map domain entity inherited base 'Id' property to DTO 'Email' field
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Id))

                // 3. Keep default true mapping for UI toggles
                .ForMember(dest => dest.IsDarkMode, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.IsTwoStepEnabled, opt => opt.MapFrom(_ => false));
        }
    }
}