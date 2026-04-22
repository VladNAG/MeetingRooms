namespace MeetingRooms.Contracts.Responses.Booking;

public class BookingSummaryResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string Status { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
}
