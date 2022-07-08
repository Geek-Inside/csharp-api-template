using Newtonsoft.Json;

namespace CSharpAPITemplate.Infrastructure.Results.Base;

/// <summary>
/// Абстракция результата бизнес-операции, с возвращаемой моделью
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T> : BaseResult, IResult<T>
{
    #region Constructor

    public Result(T data) : this()
    {
        Data = data;
    }

    public Result() { }

    #endregion

    /// <summary>
    /// Модель результата бизнес-операции
    /// </summary>
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public T? Data { get; set; }
}