using Microsoft.AspNetCore.Mvc;

namespace CSharpAPITemplate.Infrastructure.Results.Base;

public interface IResult<T> : IBaseResult
{
	/// <summary>
	/// Модель результата бизнес-операции
	/// </summary>
	T? Data { get; set; }

	IActionResult ToActionResult();
}