using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Domain.Entities;
using MediatR;

namespace MeetingRooms.Application.Commands.Bookings.CancelBooking;

public class CancelBookingCommandHandler(
    IBookingRepository bookings,
    IUserContext userContext) : IRequestHandler<CancelBookingCommand>
{
    public async Task Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var booking = await bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw NotFoundException.For<BookingRequest>(request.BookingId);

        if (booking.RequestedByUserId != userContext.UserId)
            throw new ForbiddenException();

        booking.Cancel(request.Reason, userContext.UserId);
        await bookings.SaveAsync(ct);
    }
}
