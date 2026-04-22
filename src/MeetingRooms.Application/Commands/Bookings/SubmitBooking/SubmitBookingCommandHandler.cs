using MediatR;
using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Extensions;
using MeetingRooms.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace MeetingRooms.Application.Commands.Bookings.SubmitBooking;

public class SubmitBookingCommandHandler(
    IBookingRepository bookings,
    IRoomRepository rooms,
    ILogger<SubmitBookingCommandHandler> logger) : IRequestHandler<SubmitBookingCommand>
{
    public async Task Handle(SubmitBookingCommand request, CancellationToken ct)
    {
        var booking = await bookings.GetByIdAsync(request.BookingId, ct);
            booking.EnsureExists(request.BookingId);

        UserValidationExtensions.RequireOwnership(request.UserId, booking.RequestedByUserId);

        var room = await rooms.GetByIdAsync(booking.RoomId, ct);
            room.EnsureExists(booking.RoomId);

        if (!room.IsActive)
            throw new DomainException("Room is not active.");

        if (await bookings.HasConflictAsync(booking.RoomId, booking.TimeSlot, booking.Id, ct))
            throw new DomainException("The time slot conflicts with an existing booking. Change time or room");

        booking.Submit(request.UserId);
        await bookings.SaveAsync(ct);

        logger.LogInformation("Booking submitted: BookingId={BookingId}, UserId={UserId}",
            booking.Id, request.UserId);
    }
}
