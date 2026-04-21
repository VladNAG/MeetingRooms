using MeetingRooms.Domain.Exceptions;

namespace MeetingRooms.Domain.Entities;

public sealed class TimeSlot
{
    public DateTime StartAt { get; }
    public DateTime EndAt { get; }

    public TimeSlot(DateTime startAt, DateTime endAt)
    {
        if (endAt <= startAt)
            throw new DomainException("EndAt must be after StartAt.");

        if ((endAt - startAt).TotalHours > 4)
            throw new DomainException("Booking duration cannot exceed 4 hours.");

        StartAt = startAt;
        EndAt = endAt;
    }

    public bool OverlapsWith(TimeSlot other) =>
        StartAt < other.EndAt && EndAt > other.StartAt;
}
