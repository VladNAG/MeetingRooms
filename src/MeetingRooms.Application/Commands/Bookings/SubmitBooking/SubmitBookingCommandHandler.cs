using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Domain.Entities;
using MeetingRooms.Domain.Exceptions;
using MediatR;

namespace MeetingRooms.Application.Commands.Bookings.SubmitBooking;

public class SubmitBookingCommandHandler(
    IBookingRepository bookings,
    IRoomRepository rooms,
    IUserContext userContext) : IRequestHandler<SubmitBookingCommand>
{
    public async Task Handle(SubmitBookingCommand request, CancellationToken ct)
    {
        var booking = await bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw NotFoundException.For<BookingRequest>(request.BookingId);

        if (booking.RequestedByUserId != userContext.UserId)
            throw new ForbiddenException();

        var room = await rooms.GetByIdAsync(booking.RoomId, ct)
            ?? throw NotFoundException.For<Room>(booking.RoomId);

        if (!room.IsActive)
            throw new DomainException("Room is not active.");

        if (await bookings.HasConflictAsync(booking.RoomId, booking.TimeSlot, booking.Id, ct))
            throw new DomainException("The time slot conflicts with an existing booking.");

        booking.Submit(userContext.UserId);
        await bookings.SaveAsync(ct);
    }
}
