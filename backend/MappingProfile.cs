using AutoMapper;
using backend.Entities;
using backend.Models;

namespace backend
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<RegisterUserDto, User>();
            CreateMap<backend.Entities.File, FileModelDto>();
        }


    }
}
