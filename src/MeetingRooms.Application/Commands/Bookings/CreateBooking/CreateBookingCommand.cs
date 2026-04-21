using MediatR;
using MeetingRooms.Contracts.Responses.Booking;

namespace MeetingRooms.Application.Commands.Bookings.CreateBooking;

public class CreateBookingCommand : IRequest<BookingDetailsResponse>
{
    public Guid RoomId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Purpose { get; set; } = null!;
    public List<string> Attendees { get; set; } = null!;
}
