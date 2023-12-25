using AutoMapper;
using MyOwnSummary_API.Models;
using MyOwnSummary_API.Models.Dtos.CategoryDtos;
using MyOwnSummary_API.Models.Dtos.LanguageDtos;
using MyOwnSummary_API.Models.Dtos.RoleDtos;
using MyOwnSummary_API.Models.Dtos.UserDtos;

namespace MyOwnSummary_API.Profiles
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, User>();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<Role,RoleDto>().ReverseMap(); 
            CreateMap<CreateRoleDto, Role>();
            CreateMap<Language, LanguageDto>().ReverseMap();
            CreateMap<CreateLanguageDto, Language>();
        }
    }
}
