using MediatR;
using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Mappings;
using MeetingRooms.Contracts.Responses.Room;

namespace MeetingRooms.Application.Queries.Rooms.GetRooms;

public class GetRoomsQueryHandler(IRoomRepository rooms) : IRequestHandler<GetRoomsQuery, List<RoomResponse>>
{
    public async Task<List<RoomResponse>> Handle(GetRoomsQuery request, CancellationToken ct)
    {
        var filter = new RoomFilter { Location = request.Location, MinCapacity = request.MinCapacity, IsActive = request.IsActive };
        var result = await rooms.GetFilteredAsync(filter, ct);
        return result.Select(r => r.ToDto()).ToList();
    }
}
