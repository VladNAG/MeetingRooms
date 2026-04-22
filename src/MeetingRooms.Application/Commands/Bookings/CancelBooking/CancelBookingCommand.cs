using MediatR;

namespace MeetingRooms.Application.Commands.Bookings.CancelBooking;

public class CancelBookingCommand : IRequest
{
    public Guid BookingId { get; set; }
    public string Reason { get; set; }
    public Guid UserId { get; set; }
}
