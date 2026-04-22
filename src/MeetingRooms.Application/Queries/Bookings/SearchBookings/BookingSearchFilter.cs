using MeetingRooms.Domain.Enums;

namespace MeetingRooms.Application.Queries.Bookings.SearchBookings;

public class BookingSearchFilter
{
    public DateTimeOffset? From { get; set; }
    public DateTimeOffset? To { get; set; }
    public Guid? RoomId { get; set; }
    public BookingStatus? Status { get; set; }
}
