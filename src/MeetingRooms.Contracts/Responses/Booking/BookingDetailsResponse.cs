namespace MeetingRooms.Contracts.Responses.Booking;

public class BookingDetailsResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string Status { get; set; }
    public string Purpose { get; set; }
    public List<string> Attendees { get; set; } = [];
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public Guid RequestedByUserId { get; set; }
    public List<StatusTransitionResponse> History { get; set; } = [];
}
