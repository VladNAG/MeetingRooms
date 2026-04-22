using MeetingRooms.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using MeetingRooms.Application.Extensions;

namespace MeetingRooms.Application.Commands.Bookings.ConfirmBooking;

public class ConfirmBookingCommandHandler(
    IBookingRepository bookings,
    ILogger<ConfirmBookingCommandHandler> logger) : IRequestHandler<ConfirmBookingCommand>
{
    public async Task Handle(ConfirmBookingCommand request, CancellationToken ct)
    {

        var booking = await bookings.GetByIdAsync(request.BookingId, ct);
        booking.EnsureExists(request.BookingId);

        booking.Confirm(request.UserId);
        await bookings.SaveAsync(ct);

        logger.LogInformation("Booking confirmed: BookingId={BookingId}, AdminId={AdminId}",
            booking.Id, request.UserId);
    }
}
