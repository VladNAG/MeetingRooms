using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Contracts.Enums;
using MeetingRooms.Domain.Entities;
using MediatR;

namespace MeetingRooms.Application.Commands.Bookings.ConfirmBooking;

public class ConfirmBookingCommandHandler(
    IBookingRepository bookings,
    IUserContext userContext) : IRequestHandler<ConfirmBookingCommand>
{
    public async Task Handle(ConfirmBookingCommand request, CancellationToken ct)
    {
        if (userContext.Role != UserRole.Admin)
            throw new ForbiddenException();

        var booking = await bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw NotFoundException.For<BookingRequest>(request.BookingId);

        booking.Confirm(userContext.UserId);
        await bookings.SaveAsync(ct);
    }
}
