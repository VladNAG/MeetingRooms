using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Contracts.Enums;
using Microsoft.AspNetCore.Http;

namespace MeetingRooms.Infrastructure.UserContext;

public class HttpUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private IHeaderDictionary Headers => httpContextAccessor.HttpContext!.Request.Headers;

    public Guid UserId =>
        Guid.TryParse(Headers["X-User-Id"].FirstOrDefault(), out var id)
            ? id
            : throw new ForbiddenException("X-User-Id header is missing or invalid.");

    public UserRole Role =>
        Enum.TryParse<UserRole>(Headers["X-User-Role"].FirstOrDefault(), out var role)
            ? role
            : throw new ForbiddenException("X-User-Role header is missing or invalid.");
}
