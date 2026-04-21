using MediatR;
using MeetingRooms.Contracts.Responses.Room;

namespace MeetingRooms.Application.Queries.Rooms.GetRooms;

public class GetRoomsQuery : IRequest<List<RoomResponse>>
{
    public string? Location { get; set; }
    public int? MinCapacity { get; set; }
    public bool? IsActive { get; set; }
}
