using AutoMapper;
using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Data;
using CSharpAPITemplate.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CSharpAPITemplate.BusinessLayer.Services.Comments;

public class CommentService : BaseService<Comment, CommentDto>, ICommentService
{
	public CommentService(IApplicationDbContext database, IMapper mapper, ILogger<CommentService> logger) 
		: base(database, mapper, logger)
	{ }
}