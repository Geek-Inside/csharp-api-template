using AutoMapper;
using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Domain.Entities;

namespace CSharpAPITemplate.BusinessLayer.Mappings;

public class CommentProfile : Profile
{
	public CommentProfile()
	{
		CreateMap<Comment, CommentDto>().ReverseMap();
	}
}