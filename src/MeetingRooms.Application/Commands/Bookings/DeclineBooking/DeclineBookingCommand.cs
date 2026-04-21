using MediatR;

namespace MeetingRooms.Application.Commands.Bookings.DeclineBooking;

public class DeclineBookingCommand : IRequest
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; } = null!;
}
