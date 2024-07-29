using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Shared.DTO;

namespace BusinessLayer.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (CustomException ex)
        {
            _logger.LogError($"Custom error occurred: {ex.Message}");
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task<Task> HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        int statusCode;
        string message;
        var errors = new Dictionary<string, List<string>>();

        if (exception is CustomException customException)
        {
            statusCode = customException.StatusCode;
            message = customException.Message;
        }
        else
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
            message = "Internal Server Error from the custom middleware.";
        }

        errors.Add("messages", new List<string> {message});
        context.Response.StatusCode = statusCode;
        
        return context.Response.WriteAsJsonAsync(new ErrorResponse()
        {
            StatusCode = statusCode,
            Errors = errors
        });
    }
}