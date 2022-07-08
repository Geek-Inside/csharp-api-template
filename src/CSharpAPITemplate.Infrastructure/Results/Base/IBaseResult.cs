namespace CSharpAPITemplate.Infrastructure.Results.Base;

public interface IBaseResult
{
    /// <summary>
    /// Является ли операция успешной
    /// </summary>
    bool IsSuccessStatusCode { get; }

    /// <summary>
    /// Результат операции в виде кода статуса HTTP
    /// </summary>
    int StatusCode { get; set; }
        
    /// <summary>
    /// Модель ошибки, есть операция не успешна
    /// </summary>
    public ErrorResult? Errors { get; set; }
}