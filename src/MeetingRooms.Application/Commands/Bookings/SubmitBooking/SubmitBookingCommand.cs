using MediatR;

namespace MeetingRooms.Application.Commands.Bookings.SubmitBooking;

public record SubmitBookingCommand() : IRequest
{
    public Guid UserId { get; set; }
    public Guid BookingId { get; init; }
}
