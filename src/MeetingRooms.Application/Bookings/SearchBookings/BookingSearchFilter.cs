using MeetingRooms.Domain.Enums;

namespace MeetingRooms.Application.Bookings.SearchBookings;

public record BookingSearchFilter(DateTime? From, DateTime? To, Guid? RoomId, BookingStatus? Status);
