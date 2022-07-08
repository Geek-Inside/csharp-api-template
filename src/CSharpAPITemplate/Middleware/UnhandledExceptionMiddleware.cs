using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CSharpAPITemplate.Infrastructure.Middleware;

/// <summary>
/// Middleware for logging and handling unhandled exceptions.
/// </summary>
public class UnhandledExceptionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<UnhandledExceptionMiddleware> _logger;
	public UnhandledExceptionMiddleware(RequestDelegate next, ILogger<UnhandledExceptionMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task Invoke(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception exceptionObj)
		{
			await HandleExceptionAsync(context, exceptionObj, _logger);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<UnhandledExceptionMiddleware> logger)
	{
		logger.LogError(exception.Message);

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
		return context.Response.WriteAsync(JsonConvert.SerializeObject(new { StatusCode = (int)HttpStatusCode.InternalServerError, ErrorMessage = exception.Message }));
	}
}