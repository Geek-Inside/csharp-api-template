using CSharpAPITemplate.Domain.Common.Interfaces;

namespace CSharpAPITemplate.BusinessLayer.Extensions;

public static class QueryableExtensions
{
	public static IQueryable<TEntity> OnlyActive<TEntity>(this IQueryable<TEntity> source) where TEntity : ISoftRemovable
	{
		return source.Where(s => !s.IsDeleted);
	}
}