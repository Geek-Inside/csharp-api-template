using System.ComponentModel.DataAnnotations;
using CSharpAPITemplate.Domain.Common;

namespace CSharpAPITemplate.Domain.Entities;

/// <summary>
/// Represents post that contains text and can be commented on.
/// </summary>
public class Post : BaseEntity
{
	/// <summary>
	/// Main text in post.
	/// </summary>
	[Required]
	public string Text { get; set; }
}