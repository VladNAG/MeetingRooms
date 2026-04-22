using MediatR;
using MeetingRooms.Contracts.Responses.Booking;

namespace MeetingRooms.Application.Commands.Bookings.CreateBooking;

public class CreateBookingCommand : IRequest<BookingDetailsResponse>
{
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string Purpose { get; set; } = null!;
    public List<string> Attendees { get; set; } = null!;
}
