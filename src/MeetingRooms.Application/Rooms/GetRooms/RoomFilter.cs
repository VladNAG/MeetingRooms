namespace MeetingRooms.Application.Rooms.GetRooms;

public record RoomFilter(string? Location, int? MinCapacity, bool? IsActive);
