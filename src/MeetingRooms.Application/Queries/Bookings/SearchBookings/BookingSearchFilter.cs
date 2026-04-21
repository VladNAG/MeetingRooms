using MeetingRooms.Domain.Enums;

namespace MeetingRooms.Application.Queries.Bookings.SearchBookings;

public class BookingSearchFilter
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public Guid? RoomId { get; set; }
    public BookingStatus? Status { get; set; }
}
