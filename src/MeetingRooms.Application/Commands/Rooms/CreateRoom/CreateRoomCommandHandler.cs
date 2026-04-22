using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Mappings;
using MeetingRooms.Domain.Entities;
using MediatR;
using MeetingRooms.Contracts.Responses.Room;
using Microsoft.Extensions.Logging;

namespace MeetingRooms.Application.Commands.Rooms.CreateRoom;

public class CreateRoomCommandHandler(
    IRoomRepository rooms,
    ILogger<CreateRoomCommandHandler> logger) : IRequestHandler<CreateRoomCommand, RoomResponse>
{
    public async Task<RoomResponse> Handle(CreateRoomCommand request, CancellationToken ct)
    {
        var room = Room.Create(request.Name, request.Capacity, request.Location);

        await rooms.AddAsync(room, ct);
        await rooms.SaveAsync(ct);

        logger.LogInformation("Room created: RoomId={RoomId}", room.Id);
        return room.ToDto();
    }
}
