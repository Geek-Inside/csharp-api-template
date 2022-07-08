using System.ComponentModel.DataAnnotations;
using CSharpAPITemplate.Domain.Entities;

namespace CSharpAPITemplate.BusinessLayer.Models;

/// <inheritdoc cref="Post"/>
public class PostDto : BaseEntityDto
{
	/// <inheritdoc cref="Post.Text"/>
	[Required]
	public string Text { get; set; }
}