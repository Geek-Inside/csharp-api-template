using AutoMapper;
using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Domain.Entities;

namespace CSharpAPITemplate.BusinessLayer.Mappings;

public class PostProfile : Profile
{
	public PostProfile()
	{
		CreateMap<Post, PostDto>().ReverseMap();
	}
}