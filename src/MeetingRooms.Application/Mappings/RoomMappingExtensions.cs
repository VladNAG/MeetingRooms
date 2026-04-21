using MeetingRooms.Contracts.Responses.Room;
using MeetingRooms.Domain.Entities;

namespace MeetingRooms.Application.Mappings;

public static class RoomMappingExtensions
{
    public static RoomResponse ToDto(this Room r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Capacity = r.Capacity,
        Location = r.Location,
        IsActive = r.IsActive
    };
}
