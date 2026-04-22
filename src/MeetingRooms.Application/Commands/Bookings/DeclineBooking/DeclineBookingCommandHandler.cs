using MeetingRooms.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using MeetingRooms.Application.Extensions;

namespace MeetingRooms.Application.Commands.Bookings.DeclineBooking;

public class DeclineBookingCommandHandler(
    IBookingRepository bookings,
    ILogger<DeclineBookingCommandHandler> logger) : IRequestHandler<DeclineBookingCommand>
{
    public async Task Handle(DeclineBookingCommand request, CancellationToken ct)
    {
        var booking = await bookings.GetByIdAsync(request.BookingId, ct);
           booking.EnsureExists(request.BookingId);

        booking.Decline(request.Reason, request.UserId);
        await bookings.SaveAsync(ct);

        logger.LogInformation("Booking declined: BookingId={BookingId}, AdminId={AdminId}",
            booking.Id, request.UserId);
    }
}
