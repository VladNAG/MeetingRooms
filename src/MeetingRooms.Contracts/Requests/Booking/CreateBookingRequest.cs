namespace MeetingRooms.Contracts.Requests.Booking;

public class CreateBookingRequest
{
    public Guid RoomId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public string Purpose { get; set; }
    public List<string> Attendees { get; set; } = [];
}
