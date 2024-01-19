using AutoMapper;
using userMS.Application.DTOs;
using userMS.Domain.Entities;

namespace userMS.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();

            CreateMap<RegisterUserDto, User>().ReverseMap();

            CreateMap<User, LoginResponseDto>().ReverseMap();
        }
    }
}
