namespace MeetingRooms.Contracts.Responses.Booking;

public class StatusTransitionResponse
{
    public string FromStatus { get; set; }
    public string ToStatus { get; set; }
    public Guid ChangedByUserId { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
    public string Reason { get; set; }
}
