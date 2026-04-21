using MediatR;
using MeetingRooms.Contracts.Responses.Room;

namespace MeetingRooms.Application.Commands.Rooms.CreateRoom;

public class CreateRoomCommand : IRequest<RoomResponse>
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public string Location { get; set; } = null!;
}
