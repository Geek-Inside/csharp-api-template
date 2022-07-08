using AutoMapper;
using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Domain.Entities;

namespace CSharpAPITemplate.BusinessLayer.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
	}
}