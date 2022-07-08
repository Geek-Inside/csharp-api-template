using CSharpAPITemplate.Domain.Common;
using CSharpAPITemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CSharpAPITemplate.Data;

/// <summary>
/// Extends <see cref="DbContext"/> with application context.
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
	/// <summary>
	/// Parameterless constructor for mocking in unit tests.
	/// </summary>
	public ApplicationDbContext()
	{
	}
        
	public ApplicationDbContext(DbContextOptions opt)
		: base(opt)
	{
	}
	
	public DbSet<Post> Posts { get; set; }
	
	public DbSet<Comment> Comments { get; set; }

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		SetupBaseObjectProperties();
		return base.SaveChangesAsync(cancellationToken);
	}

	/// <summary>
	/// Sets <see cref="BaseEntity"/> properties on saving changes.
	/// </summary>
	private void SetupBaseObjectProperties()
	{
		foreach (var entry in ChangeTracker.Entries())
		{
			if (entry.Entity is not BaseEntity entityObject)
				continue;

			switch (entry.State)
			{
				case EntityState.Detached:
				case EntityState.Unchanged:
				case EntityState.Modified:
				case EntityState.Added:
					break;
				case EntityState.Deleted:
					entityObject.IsDeleted = true;
					entityObject.DeleteDateTime = DateTime.UtcNow;
					break;
			}
		}
	}
}