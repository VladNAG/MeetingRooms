using MediatR;

namespace MeetingRooms.Application.Commands.Bookings.SubmitBooking;

public class SubmitBookingCommand : IRequest
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
}
