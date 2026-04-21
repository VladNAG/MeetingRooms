using MeetingRooms.Domain.Enums;
using MeetingRooms.Domain.Exceptions;

namespace MeetingRooms.Domain.Entities;

public class BookingRequest
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public TimeSlot TimeSlot { get; private set; } = default!;
    public string Purpose { get; private set; } = default!;
    public List<string> Attendees { get; private set; } = [];
    public BookingStatus Status { get; private set; }

    private readonly List<StatusTransition> _transitions = [];
    public IReadOnlyList<StatusTransition> Transitions => _transitions.AsReadOnly();

    private BookingRequest() { }

    public static BookingRequest Create(
        Guid roomId,
        Guid requestedByUserId,
        TimeSlot timeSlot,
        string purpose,
        List<string> attendees)
    {
        if (string.IsNullOrWhiteSpace(purpose))
            throw new DomainException("Purpose is required.");

        if (attendees is null || attendees.Count == 0)
            throw new DomainException("At least one attendee is required.");

        return new BookingRequest
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            RequestedByUserId = requestedByUserId,
            TimeSlot = timeSlot,
            Purpose = purpose,
            Attendees = attendees,
            Status = BookingStatus.Draft
        };
    }

    public void Submit(Guid byUserId)
    {
        if (Status != BookingStatus.Draft)
            throw new DomainException($"Cannot submit booking in '{Status}' status.");

        AddTransition(BookingStatus.Draft, BookingStatus.Submitted, byUserId);
        Status = BookingStatus.Submitted;
    }

    public void Confirm(Guid byUserId)
    {
        if (Status != BookingStatus.Submitted)
            throw new DomainException($"Cannot confirm booking in '{Status}' status.");

        AddTransition(BookingStatus.Submitted, BookingStatus.Confirmed, byUserId);
        Status = BookingStatus.Confirmed;
    }

    public void Decline(string reason, Guid byUserId)
    {
        if (Status != BookingStatus.Submitted)
            throw new DomainException($"Cannot decline booking in '{Status}' status.");

        AddTransition(BookingStatus.Submitted, BookingStatus.Declined, byUserId, reason);
        Status = BookingStatus.Declined;
    }

    public void Cancel(string reason, Guid byUserId)
    {
        if (Status != BookingStatus.Confirmed)
            throw new DomainException($"Cannot cancel booking in '{Status}' status.");

        AddTransition(BookingStatus.Confirmed, BookingStatus.Cancelled, byUserId, reason);
        Status = BookingStatus.Cancelled;
    }

    private void AddTransition(BookingStatus from, BookingStatus to, Guid byUserId, string? reason = null) =>
        _transitions.Add(StatusTransition.Create(Id, from, to, byUserId, reason));
}
