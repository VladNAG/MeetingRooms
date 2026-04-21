using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Bookings.SearchBookings;
using MeetingRooms.Domain.Entities;
using MeetingRooms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MeetingRooms.DataAccess.Repositories;

public class BookingRepository(MeetingRoomsDbContext context) : IBookingRepository
{
    public Task<BookingRequest?> GetByIdAsync(Guid id, CancellationToken ct) =>
        context.BookingRequests
            .Include(b => b.Transitions)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

    public Task<List<BookingRequest>> SearchAsync(BookingSearchFilter filter, CancellationToken ct)
    {
        var query = context.BookingRequests.AsQueryable();

        if (filter.RoomId.HasValue)
            query = query.Where(b => b.RoomId == filter.RoomId.Value);

        if (filter.Status.HasValue)
            query = query.Where(b => b.Status == filter.Status.Value);

        if (filter.From.HasValue)
            query = query.Where(b => b.TimeSlot.EndAt >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(b => b.TimeSlot.StartAt <= filter.To.Value);

        return query.ToListAsync(ct);
    }

    public Task<bool> HasConflictAsync(Guid roomId, TimeSlot slot, Guid? excludeBookingId, CancellationToken ct) =>
        context.BookingRequests
            .Where(b => b.RoomId == roomId
                && (excludeBookingId == null || b.Id != excludeBookingId)
                && (b.Status == BookingStatus.Submitted || b.Status == BookingStatus.Confirmed)
                && b.TimeSlot.StartAt < slot.EndAt
                && b.TimeSlot.EndAt > slot.StartAt)
            .AnyAsync(ct);

    public async Task AddAsync(BookingRequest booking, CancellationToken ct) =>
        await context.BookingRequests.AddAsync(booking, ct);

    public Task SaveAsync(CancellationToken ct) =>
        context.SaveChangesAsync(ct);
}
