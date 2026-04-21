using FluentValidation;

namespace MeetingRooms.Application.Queries.Bookings.GetBooking;

public class GetBookingQueryValidator : AbstractValidator<GetBookingQuery>
{
    public GetBookingQueryValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
    }
}
