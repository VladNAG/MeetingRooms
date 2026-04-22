using MediatR;
using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Mappings;
using MeetingRooms.Contracts.Responses.Booking;

namespace MeetingRooms.Application.Queries.Bookings.SearchBookings;

public class SearchBookingsQueryHandler(IBookingRepository bookings) : IRequestHandler<SearchBookingsQuery, List<BookingSummaryResponse>>
{
    public async Task<List<BookingSummaryResponse>> Handle(SearchBookingsQuery request, CancellationToken ct)
    {
        var filter = new BookingSearchFilter { From = request.From, To = request.To, RoomId = request.RoomId, Status = request.Status };
        var result = await bookings.SearchAsync(filter, ct);

        return result.Select(b => b.ToSummaryDto()).ToList();
    }
}
