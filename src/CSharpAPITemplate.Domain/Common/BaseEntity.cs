using System.ComponentModel.DataAnnotations;
using CSharpAPITemplate.Domain.Common.Interfaces;

namespace CSharpAPITemplate.Domain.Common;

/// <summary>
/// Base implementation of typical database entity.
/// </summary>
public abstract class BaseEntity : IEntity, ISoftRemovable
{
	[Key]
	public long Id { get; set; }
	public bool IsDeleted { get; set; }
	public DateTime? DeleteDateTime { get; set; }
}