namespace CSharpAPITemplate.BusinessLayer.Models;

/// <summary>
/// Base implementation of typical database entity.
/// </summary>
public abstract class BaseEntityDto
{
	public long Id { get; set; }
	public bool IsDeleted { get; set; }
	public DateTime? DeleteDateTime { get; set; }
}