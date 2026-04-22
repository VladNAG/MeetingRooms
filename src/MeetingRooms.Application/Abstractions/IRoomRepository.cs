using MeetingRooms.Application.Queries.Rooms.GetRooms;
using MeetingRooms.Domain.Entities;

namespace MeetingRooms.Application.Abstractions;

public interface IRoomRepository
{
    Task<Room> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Room>> GetFilteredAsync(RoomFilter filter, CancellationToken ct);
    Task AddAsync(Room room, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
