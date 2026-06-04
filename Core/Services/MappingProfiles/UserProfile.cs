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
                .ConstructUsing(src => new UserDto(
                    src.UserInternalId,
                    src.FullName,
                    src.Id,
                    src.IsMonthlyReportEnabled,
                    false,
                    false));
        }
    }
}