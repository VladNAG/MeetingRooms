namespace MeetingRooms.Contracts.Requests.Room;

public class CreateRoomRequest
{
    public string Name { get; set; }
    public int Capacity { get; set; }
    public string Location { get; set; }
}
