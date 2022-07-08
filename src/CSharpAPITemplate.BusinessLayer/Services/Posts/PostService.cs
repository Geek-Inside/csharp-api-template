using AutoMapper;
using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Data;
using CSharpAPITemplate.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CSharpAPITemplate.BusinessLayer.Services.Posts;

public class PostService : BaseService<Post, PostDto>, IPostService
{
	public PostService(IApplicationDbContext database, IMapper mapper, ILogger logger) : base(database, mapper, logger)
	{
	}
}