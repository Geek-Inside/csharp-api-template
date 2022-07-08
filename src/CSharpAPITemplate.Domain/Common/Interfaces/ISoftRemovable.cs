namespace CSharpAPITemplate.Domain.Common.Interfaces;

/// <summary>
/// Defines properties and method for implementing soft delete pattern.
/// </summary>
public interface ISoftRemovable
{
	/// <summary>
	/// Flag object as deleted or not.
	/// </summary>
	public bool IsDeleted { get; set; }
	
	/// <summary>
	/// Last delete <see cref="DateTime"/>. Null if the object was never deleted.
	/// </summary>
	public DateTime? DeleteDateTime { get; set; }
}