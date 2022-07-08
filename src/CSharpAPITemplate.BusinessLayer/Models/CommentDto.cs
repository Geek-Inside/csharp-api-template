using System.ComponentModel.DataAnnotations;
using CSharpAPITemplate.Domain.Entities;

namespace CSharpAPITemplate.BusinessLayer.Models;

/// <inheritdoc cref="Comment"/>
public class CommentDto : BaseEntityDto
{
	/// <inheritdoc cref="Comment.PostId"/>
	[Required]
	public long PostId { get; set; }
	
	/// <inheritdoc cref="Comment.Text"/>
	[Required]
	public string Text { get; set; }
}