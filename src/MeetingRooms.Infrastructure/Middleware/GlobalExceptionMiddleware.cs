using System.Text.Json;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MeetingRooms.Infrastructure.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private Task HandleAsync(HttpContext context, Exception ex)
    {
        var traceId = context.TraceIdentifier;
        var method = context.Request.Method;
        var path = context.Request.Path;

        var (status, message) = ex switch
        {
            NotFoundException e  => (StatusCodes.Status404NotFound, e.Message),
            ForbiddenException   => (StatusCodes.Status403Forbidden, "Access denied."),
            DomainException e    => (StatusCodes.Status422UnprocessableEntity, e.Message),
            _                    => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        switch (status)
        {
            case StatusCodes.Status500InternalServerError:
                logger.LogError(ex,
                    "Unhandled exception: TraceId={TraceId}, {Method} {Path}",
                    traceId, method, path);
                break;
            case StatusCodes.Status422UnprocessableEntity:
                logger.LogWarning(
                    "Domain rule violation: TraceId={TraceId}, {Method} {Path}, Error={Error}",
                    traceId, method, path, ex.Message);
                break;
            case StatusCodes.Status404NotFound:
                logger.LogWarning(
                    "Not found: TraceId={TraceId}, {Method} {Path}, Error={Error}",
                    traceId, method, path, ex.Message);
                break;
            case StatusCodes.Status403Forbidden:
                logger.LogWarning(
                    "Forbidden: TraceId={TraceId}, {Method} {Path}",
                    traceId, method, path);
                break;
        }

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new { traceId, error = message }, JsonOptions);
        return context.Response.WriteAsync(body);
    }
}
