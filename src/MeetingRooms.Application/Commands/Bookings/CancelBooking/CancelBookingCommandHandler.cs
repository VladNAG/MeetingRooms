using MeetingRooms.Application.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using MeetingRooms.Application.Extensions;

namespace MeetingRooms.Application.Commands.Bookings.CancelBooking;

public class CancelBookingCommandHandler(
    IBookingRepository bookings,
    ILogger<CancelBookingCommandHandler> logger) : IRequestHandler<CancelBookingCommand>
{
    public async Task Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var booking = await bookings.GetByIdAsync(request.BookingId, ct);
        booking.EnsureExists(request.BookingId);

        UserValidationExtensions.RequireOwnership(request.UserId, booking.RequestedByUserId);

        booking.Cancel(request.Reason, request.UserId);
        await bookings.SaveAsync(ct);

        logger.LogInformation("Booking cancelled: BookingId={BookingId}, UserId={UserId}",
            booking.Id, request.UserId);
    }
}
