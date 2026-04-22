using MediatR;
using MeetingRooms.Application.Abstractions;
using MeetingRooms.Application.Commands.Rooms.CreateRoom;
using MeetingRooms.Application.Queries.Rooms.GetRooms;
using MeetingRooms.Contracts.Enums;
using MeetingRooms.Contracts.Requests.Room;
using MeetingRooms.Contracts.Responses.Room;
using Microsoft.AspNetCore.Mvc;
using MeetingRooms.Application.Extensions;

namespace MeetingRooms.WebApi.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController(IMediator mediator, IUserContext userContext) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(RoomResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequest request, CancellationToken ct)
    {
        userContext.RequireRole(UserRole.Admin);

        var command = new CreateRoomCommand
        {
          Name = request.Name,
          Capacity = request.Capacity,
          Location = request.Location
        };

        var room = await mediator.Send(command, ct);

        return CreatedAtAction(nameof(GetAll), new { }, room);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<RoomResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetRoomsRequest request, CancellationToken ct)
    {
        var query = new GetRoomsQuery
        {
           Location = request.Location,
           IsActive = request.IsActive,
           MinCapacity = request.MinCapacity,
        };

        var rooms = await mediator.Send(query, ct);
        return Ok(rooms);
    }
}
