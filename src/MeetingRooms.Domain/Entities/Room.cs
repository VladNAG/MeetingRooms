using MeetingRooms.Domain.Exceptions;

namespace MeetingRooms.Domain.Entities;

public class Room
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public int Capacity { get; private set; }
    public string Location { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Room() { }

    public static Room Create(string name, int capacity, string location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Room name is required.");

        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("Room location is required.");

        if (capacity <= 0)
            throw new DomainException("Room capacity must be greater than zero.");

        return new Room
        {
            Id = Guid.NewGuid(),
            Name = name,
            Capacity = capacity,
            Location = location,
            IsActive = true
        };
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
