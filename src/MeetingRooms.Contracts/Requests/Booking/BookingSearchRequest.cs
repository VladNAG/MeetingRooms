using MeetingRooms.Contracts.Enums;

namespace MeetingRooms.Contracts.Requests.Booking;

public class BookingSearchRequest
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public Guid? RoomId { get; set; }
    public BookingStatusDto? Status { get; set; }
}
