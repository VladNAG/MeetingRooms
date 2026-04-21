using MediatR;

namespace MeetingRooms.Application.Commands.Bookings.SubmitBooking;

public record SubmitBookingCommand() : IRequest
{
        public Guid BookingId { get; init; }
}
