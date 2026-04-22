using MeetingRooms.DataAccess;
using MeetingRooms.Domain.Entities;
using MeetingRooms.Domain.Enums;

namespace MeetingRooms.IntegrationTests.Infrastructure;

public static class DbSeeder
{
    public static readonly DateTimeOffset SlotStart = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);
    public static readonly DateTimeOffset SlotEnd   = SlotStart.AddHours(1);

    public static async Task<Room> SeedRoomAsync(
        MeetingRoomsDbContext db,
        bool isActive = true,
        string name = "Test Room",
        int capacity = 10)
    {
        var room = Room.Create(name, capacity, "Building A");
        if (!isActive) room.Deactivate();
        db.Rooms.Add(room);
        await db.SaveChangesAsync();
        return room;
    }

    public static async Task<BookingRequest> SeedBookingAsync(
        MeetingRoomsDbContext db,
        Guid roomId,
        Guid userId,
        BookingStatus targetStatus = BookingStatus.Draft,
        DateTimeOffset? start = null,
        DateTimeOffset? end = null)
    {
        var slot    = new TimeSlot(start ?? SlotStart, end ?? SlotEnd);
        var booking = BookingRequest.Create(roomId, userId, slot, "Meeting", ["user@example.com"]);

        if (targetStatus >= BookingStatus.Submitted)
            booking.Submit(userId);
        if (targetStatus >= BookingStatus.Confirmed)
            booking.Confirm(Guid.NewGuid());

        db.BookingRequests.Add(booking);
        await db.SaveChangesAsync();
        return booking;
    }
}
