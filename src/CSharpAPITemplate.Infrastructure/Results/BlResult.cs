using CSharpAPITemplate.Infrastructure.Results.Base;

namespace CSharpAPITemplate.Infrastructure.Results;

/// <summary>
/// Статическая обобщённая обёртка, возвращающая разные виды Result
/// </summary>
public abstract class BlResult<T> 
{
    public static Result<T> Ok(T data)
    {
        return new Result<T>(data)
        {
            StatusCode = 200
        };
    }
    
    public static Result<T> NoContent()
    {
        return new Result<T>
        {
            StatusCode = 204
        };
    }

    public static Result<T> BadRequest(string errorDescription, string? errorMessage = null)
    {
        return GetResult(400, errorMessage ?? "The server cannot or will not process the request due to an apparent client error (e.g., malformed request syntax, size too large, invalid request message framing, or deceptive request routing).", errorDescription);
    }

    public static Result<T> NotFound(string errorDescription, string? errorMessage = null)
    {
        return GetResult(404, errorMessage ?? "The requested resource could not be found.", errorDescription);
    }

    public static Result<T> NotAllowed(string errorDescription, string? errorMessage = null)
    {
        return GetResult(405, errorMessage ?? "A request method is not supported for the requested resource.", errorDescription);
    }

    public static Result<T> Conflict(string errorDescription, string? errorMessage = null)
    {
        return GetResult(409, errorMessage ?? "Request could not be processed because of conflict in the current state of the resource.", errorDescription);
    }

    public static Result<T> InternalError(string errorDescription, string? errorMessage = null)
    {
        return GetResult(500, errorMessage ?? "Internal error. An unexpected condition was encountered.", errorDescription);
    }

    public static Result<T> CustomError(int statusCode, string errorDescription, string errorMessage)
    {
        return GetResult(statusCode, errorMessage, errorDescription);
    }

    private static Result<T> GetResult(int statusCode, string errorMessage, string errorDescription)
    {
        var res = new Result<T>
        {
            StatusCode = statusCode,
            Errors = new ErrorResult
            {
                Message = errorMessage,
                Description = errorDescription
            }
        };
        return res;
    }
}