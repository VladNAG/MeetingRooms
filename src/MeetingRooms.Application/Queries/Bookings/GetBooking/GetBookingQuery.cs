using MediatR;
using MeetingRooms.Contracts.Responses.Booking;

namespace MeetingRooms.Application.Queries.Bookings.GetBooking;

public class GetBookingQuery : IRequest<BookingDetailsResponse>
{
    public Guid BookingId { get; set; }
}
