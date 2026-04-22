using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Queries.Rooms.GetRooms;
using MeetingRooms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRooms.DataAccess.Repositories;

public class RoomRepository(MeetingRoomsDbContext context) : IRoomRepository
{
    public Task<Room?> GetByIdAsync(Guid id, CancellationToken ct) =>
        context.Rooms.FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<List<Room>> GetFilteredAsync(RoomFilter filter, CancellationToken ct)
    {
        var query = context.Rooms.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Location))
            query = query.Where(r => r.Location == filter.Location);

        if (filter.MinCapacity.HasValue)
            query = query.Where(r => r.Capacity >= filter.MinCapacity.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(r => r.IsActive == filter.IsActive.Value);

        return query.ToListAsync(ct);
    }

    public async Task AddAsync(Room room, CancellationToken ct) =>
        await context.Rooms.AddAsync(room, ct);

    public Task SaveAsync(CancellationToken ct) =>
        context.SaveChangesAsync(ct);
}
