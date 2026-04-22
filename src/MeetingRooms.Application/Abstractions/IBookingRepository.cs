using MeetingRooms.Application.Queries.Bookings.SearchBookings;
using MeetingRooms.Domain.Entities;

namespace MeetingRooms.Application.Abstractions;

public interface IBookingRepository
{
    Task<BookingRequest> GetByIdAsync(Guid id, CancellationToken ct);
    Task<BookingRequest> GetByIdForUpdateAsync(Guid id, CancellationToken ct);
    Task<List<BookingRequest>> SearchAsync(BookingSearchFilter filter, CancellationToken ct);
    Task<bool> HasConflictAsync(Guid roomId, TimeSlot slot, Guid? excludeBookingId, CancellationToken ct);
    Task AddAsync(BookingRequest booking, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
