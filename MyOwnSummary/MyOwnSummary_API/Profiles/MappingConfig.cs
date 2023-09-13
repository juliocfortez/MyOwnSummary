using AutoMapper;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Models.Dtos.UserDtos;

namespace MyOwnSummary_API.Profiles
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, User>();
        }
    }
}
