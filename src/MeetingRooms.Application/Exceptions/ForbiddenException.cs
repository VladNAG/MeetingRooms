namespace MeetingRooms.Application.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("Access denied.") { }
    public ForbiddenException(string message) : base(message) { }
}
