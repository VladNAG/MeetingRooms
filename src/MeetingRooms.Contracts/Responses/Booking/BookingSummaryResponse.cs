namespace MeetingRooms.Contracts.Responses.Booking;

public class BookingSummaryResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string Status { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
}
