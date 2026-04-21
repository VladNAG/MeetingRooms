using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Contracts.Enums;
using MeetingRooms.Domain.Entities;
using MediatR;

namespace MeetingRooms.Application.Commands.Bookings.DeclineBooking;

public class DeclineBookingCommandHandler(
    IBookingRepository bookings,
    IUserContext userContext) : IRequestHandler<DeclineBookingCommand>
{
    public async Task Handle(DeclineBookingCommand request, CancellationToken ct)
    {
        if (userContext.Role != UserRole.Admin)
            throw new ForbiddenException();

        var booking = await bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw NotFoundException.For<BookingRequest>(request.BookingId);

        booking.Decline(request.Reason, userContext.UserId);
        await bookings.SaveAsync(ct);
    }
}
