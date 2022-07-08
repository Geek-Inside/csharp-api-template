using Newtonsoft.Json;

namespace CSharpAPITemplate.Infrastructure.Results.Base;

/// <summary>
/// Базовый класс для всех объектов Result
/// </summary>
public class BaseResult : IBaseResult
{
    /// <summary>
    /// Является ли операция успешной
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsSuccessStatusCode =>  StatusCode is >= 200 and <= 299; 
        
    /// <summary>
    /// Результат операции в виде кода статуса HTTP
    /// </summary>
    [JsonProperty("status_code")]
    public int StatusCode { get; set; }

    /// <summary>
    /// Модель ошибки, есть операция не успешна
    /// </summary>
    [JsonProperty("errors", NullValueHandling = NullValueHandling.Ignore)]
    public ErrorResult? Errors { get; set; }
}