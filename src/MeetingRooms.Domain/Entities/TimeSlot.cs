using MeetingRooms.Domain.Exceptions;

namespace MeetingRooms.Domain.Entities;

public sealed class TimeSlot
{
    public DateTimeOffset StartAt { get; }
    public DateTimeOffset EndAt { get; }

    public TimeSlot(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        if (endAt <= startAt)
            throw new DomainException("EndAt must be after StartAt.");

        if ((endAt - startAt).TotalHours > 4)
            throw new DomainException("Booking duration cannot exceed 4 hours.");

        StartAt = startAt;
        EndAt = endAt;
    }

}
