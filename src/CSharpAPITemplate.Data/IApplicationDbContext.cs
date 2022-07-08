using CSharpAPITemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CSharpAPITemplate.Data;

public interface IApplicationDbContext
{
	public DbSet<Post> Posts { get; set; }
	public DbSet<Comment> Comments { get; set; }
	
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	DbSet<TEntity> Set<TEntity>() where TEntity : class;
}