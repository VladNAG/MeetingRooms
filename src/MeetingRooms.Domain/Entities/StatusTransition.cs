using MeetingRooms.Domain.Enums;

namespace MeetingRooms.Domain.Entities;

public class StatusTransition
{
    public Guid Id { get; private set; }
    public Guid BookingRequestId { get; private set; }
    public BookingStatus FromStatus { get; private set; }
    public BookingStatus ToStatus { get; private set; }
    public Guid ChangedByUserId { get; private set; }
    public DateTimeOffset ChangedAt { get; private set; }
    public string? Reason { get; private set; }

    private StatusTransition() { }

    internal static StatusTransition Create(
        Guid bookingRequestId,
        BookingStatus from,
        BookingStatus to,
        Guid changedByUserId,
        string? reason = null) => new()
    {
        Id = Guid.NewGuid(),
        BookingRequestId = bookingRequestId,
        FromStatus = from,
        ToStatus = to,
        ChangedByUserId = changedByUserId,
        ChangedAt = DateTimeOffset.UtcNow,
        Reason = reason
    };
}
