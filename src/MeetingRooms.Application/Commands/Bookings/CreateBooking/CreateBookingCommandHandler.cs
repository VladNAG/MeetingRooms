using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Application.Mappings;
using MeetingRooms.Domain.Entities;
using MediatR;
using MeetingRooms.Contracts.Responses.Booking;
using Microsoft.Extensions.Logging;
using MeetingRooms.Application.Extensions;

namespace MeetingRooms.Application.Commands.Bookings.CreateBooking;

public class CreateBookingCommandHandler(
    IBookingRepository bookings,
    IRoomRepository rooms,
    ILogger<CreateBookingCommandHandler> logger) : IRequestHandler<CreateBookingCommand, BookingDetailsResponse>
{
    public async Task<BookingDetailsResponse> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var room = await rooms.GetByIdAsync(request.RoomId, ct);
            room.EnsureExists(request.RoomId);

        var slot = new TimeSlot(request.StartAt.ToUniversalTime(), request.EndAt.ToUniversalTime());

        var booking = BookingRequest.Create(room.Id, request.UserId, slot, request.Purpose, request.Attendees);

        await bookings.AddAsync(booking, ct);
        await bookings.SaveAsync(ct);

        logger.LogInformation("Booking created: BookingId={BookingId}, RoomId={RoomId}, UserId={UserId}",
            booking.Id, booking.RoomId, booking.RequestedByUserId);

        return booking.ToDto();
    }
}
