using System.ComponentModel.DataAnnotations;
using CSharpAPITemplate.Domain.Common;

namespace CSharpAPITemplate.Domain.Entities;

/// <summary>
/// Represents comments that are left under posts.
/// </summary>
public class Comment : BaseEntity
{
	/// <summary>
	/// The post under which the comment was left.
	/// </summary>
	[Required]
	public long PostId { get; set; }
	
	/// <summary>
	/// Text of comment.
	/// </summary>
	[Required]
	public string Text { get; set; }
}