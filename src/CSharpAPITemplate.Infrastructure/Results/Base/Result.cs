using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CSharpAPITemplate.Infrastructure.Results.Base;

/// <summary>
/// Абстракция результата бизнес-операции, с возвращаемой моделью
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T> : BaseResult, IResult<T>
{
    public Result(T data) : this()
    {
        Data = data;
    }

    public Result() { }

    /// <summary>
    /// Модель результата бизнес-операции
    /// </summary>
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public T? Data { get; set; }

    public IActionResult ToActionResult()
    {
        return StatusCode switch
        {
            200 => Data == null ? new OkResult() : new OkObjectResult(Data),
            204 => new NoContentResult(),
            _ => new ObjectResult(this)
            {
                StatusCode = StatusCode
            }
        };
    }

    /// <summary>
    /// Casting result to another generic type.
    /// </summary>
    public Result<TOut> ToResult<TOut>()
    {
        return new()
        {
            Errors = Errors,
            StatusCode = StatusCode
        };
    }
}