using MediatR;

namespace MeetingRooms.Application.Commands.Bookings.ConfirmBooking;

public class ConfirmBookingCommand : IRequest
{
    public Guid BookingId { get; set; }
}
