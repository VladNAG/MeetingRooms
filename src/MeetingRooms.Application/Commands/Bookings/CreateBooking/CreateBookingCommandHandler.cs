using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Application.Mappings;
using MeetingRooms.Domain.Entities;
using MediatR;
using MeetingRooms.Contracts.Responses.Booking;

namespace MeetingRooms.Application.Commands.Bookings.CreateBooking;

public class CreateBookingCommandHandler(
    IBookingRepository bookings,
    IRoomRepository rooms,
    IUserContext userContext) : IRequestHandler<CreateBookingCommand, BookingDetailsResponse>
{
    public async Task<BookingDetailsResponse> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var room = await rooms.GetByIdAsync(request.RoomId, ct)
            ?? throw NotFoundException.For<Room>(request.RoomId);

        var slot = new TimeSlot(
            DateTime.SpecifyKind(request.StartAt, DateTimeKind.Utc),
            DateTime.SpecifyKind(request.EndAt, DateTimeKind.Utc));

        var booking = BookingRequest.Create(room.Id, userContext.UserId, slot, request.Purpose, request.Attendees);

        await bookings.AddAsync(booking, ct);
        await bookings.SaveAsync(ct);
        return booking.ToDto();
    }
}
