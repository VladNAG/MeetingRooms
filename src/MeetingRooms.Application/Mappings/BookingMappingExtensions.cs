using MeetingRooms.Contracts.Responses.Booking;
using MeetingRooms.Domain.Entities;

namespace MeetingRooms.Application.Mappings;

public static class BookingMappingExtensions
{
    public static BookingDetailsResponse ToDto(this BookingRequest b) => new()
    {
        Id = b.Id,
        RoomId = b.RoomId,
        Status = b.Status.ToString(),
        Purpose = b.Purpose,
        Attendees = b.Attendees,
        StartAt = b.TimeSlot.StartAt,
        EndAt = b.TimeSlot.EndAt,
        RequestedByUserId = b.RequestedByUserId,
        History = b.Transitions.Select(t => t.ToDto()).ToList()
    };

    public static BookingSummaryResponse ToSummaryDto(this BookingRequest b) => new()
    {
        Id = b.Id,
        RoomId = b.RoomId,
        Status = b.Status.ToString(),
        StartAt = b.TimeSlot.StartAt,
        EndAt = b.TimeSlot.EndAt
    };

    public static StatusTransitionResponse ToDto(this StatusTransition t) => new()
    {
        FromStatus = t.FromStatus.ToString(),
        ToStatus = t.ToStatus.ToString(),
        ChangedByUserId = t.ChangedByUserId,
        ChangedAt = t.ChangedAt,
        Reason = t.Reason
    };
}
