namespace MeetingRooms.Contracts.Requests.Room;

public class GetRoomsRequest
{
    public string Location { get; set; }
    public int? MinCapacity { get; set; }
    public bool? IsActive { get; set; }
}
