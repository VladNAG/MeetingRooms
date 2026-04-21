namespace MeetingRooms.Contracts.Responses.Room;

public class RoomResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public string Location { get; set; }
    public bool IsActive { get; set; }
}
