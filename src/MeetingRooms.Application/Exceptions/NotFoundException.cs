namespace MeetingRooms.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public static NotFoundException For<T>(Guid id) =>
        new($"{typeof(T).Name} with id '{id}' was not found.");
}
