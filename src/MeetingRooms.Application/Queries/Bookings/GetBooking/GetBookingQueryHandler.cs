using MediatR;
using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Exceptions;
using MeetingRooms.Application.Mappings;
using MeetingRooms.Contracts.Responses.Booking;
using MeetingRooms.Domain.Entities;

namespace MeetingRooms.Application.Queries.Bookings.GetBooking;

public class GetBookingQueryHandler(IBookingRepository bookings) : IRequestHandler<GetBookingQuery, BookingDetailsResponse>
{
    public async Task<BookingDetailsResponse> Handle(GetBookingQuery request, CancellationToken ct)
    {
        var booking = await bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw NotFoundException.For<BookingRequest>(request.BookingId);

        return booking.ToDto();
    }
}
