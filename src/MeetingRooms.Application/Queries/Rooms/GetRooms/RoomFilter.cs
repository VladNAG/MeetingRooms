namespace MeetingRooms.Application.Queries.Rooms.GetRooms;

public class RoomFilter
{
    public string? Location { get; set; }
    public int? MinCapacity { get; set; }
    public bool? IsActive { get; set; }
}
