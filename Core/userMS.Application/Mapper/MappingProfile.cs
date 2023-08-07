using AutoMapper;
using userMS.Application.DTOs;
using userMS.Domain.Entities;

namespace userMS.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // since UserDto has an Id type of string and User entity
            // has an Id type of Guid we need to convert types during mapping
            // after concluding the type of Id of the User there will be no need for
            // type conversion 

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<RegisterUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)));
            CreateMap<User, RegisterUserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<User, LoginUserDto>().ReverseMap();

            CreateMap<User, LoginResponseDto>().ReverseMap();
        }
    }
}
