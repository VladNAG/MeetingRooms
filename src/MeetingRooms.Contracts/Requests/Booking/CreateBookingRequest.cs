namespace MeetingRooms.Contracts.Requests.Booking;

public class CreateBookingRequest
{
    public Guid RoomId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string Purpose { get; set; }
    public List<string> Attendees { get; set; } = [];
}
