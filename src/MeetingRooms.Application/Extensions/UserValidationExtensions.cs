using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Contracts.Enums;

namespace MeetingRooms.Application.Extensions
{
    public static class UserValidationExtensions
    {
        public static void RequireRole(this IUserContext context, params UserRole[] roles)
        {
            if (!roles.Contains(context.Role))
                throw new ForbiddenException("Insufficient permissions.");
        }

        public static void RequireOwnership( Guid userId, Guid ownerId)
        {
            if (userId != ownerId )
                throw new ForbiddenException("Not allowed to access this resource.");
        }
    }
}
