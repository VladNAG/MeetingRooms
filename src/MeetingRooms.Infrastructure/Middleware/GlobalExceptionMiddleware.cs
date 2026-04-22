using System.Text.Json;
using FluentValidation;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MeetingRooms.Infrastructure.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private static string TitleForStatus(int status) => status switch
    {
        StatusCodes.Status400BadRequest          => "Bad Request",
        StatusCodes.Status403Forbidden           => "Forbidden",
        StatusCodes.Status404NotFound            => "Not Found",
        StatusCodes.Status422UnprocessableEntity => "Unprocessable Entity",
        _                                        => "Internal Server Error"
    };

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
            ValidationException e => (StatusCodes.Status400BadRequest,
                string.Join("; ", e.Errors.Select(x => x.ErrorMessage))),
            NotFoundException e  => (StatusCodes.Status404NotFound, e.Message),
            ForbiddenException   => (StatusCodes.Status403Forbidden, "Access denied."),
            DomainException e    => (StatusCodes.Status422UnprocessableEntity, e.Message),
            _                    => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        switch (status)
        {
            case StatusCodes.Status400BadRequest:
                logger.LogWarning(
                    "Validation failed: TraceId={TraceId}, {Method} {Path}, Error={Error}",
                    traceId, method, path, ex.Message);
                break;
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
        context.Response.ContentType = "application/problem+json";

        var body = JsonSerializer.Serialize(new
        {
            type = $"https://httpstatuses.io/{status}",
            title = TitleForStatus(status),
            status,
            detail = message,
            traceId
        }, JsonOptions);
        return context.Response.WriteAsync(body);
    }
}
