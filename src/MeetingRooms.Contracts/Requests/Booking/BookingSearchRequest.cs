using MeetingRooms.Contracts.Enums;

namespace MeetingRooms.Contracts.Requests.Booking;

public class BookingSearchRequest
{
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
    public Guid? RoomId { get; set; }
    public BookingStatusDto? Status { get; set; }
}
