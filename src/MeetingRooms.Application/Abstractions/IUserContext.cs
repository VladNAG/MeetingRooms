namespace MeetingRooms.Application.Abstractions;

public interface IUserContext
{
    Guid UserId { get; }
    UserRole Role { get; }
}
