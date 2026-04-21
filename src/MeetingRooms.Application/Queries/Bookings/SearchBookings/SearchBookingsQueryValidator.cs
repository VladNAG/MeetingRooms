using FluentValidation;

namespace MeetingRooms.Application.Queries.Bookings.SearchBookings;

public class SearchBookingsQueryValidator : AbstractValidator<SearchBookingsQuery>
{
    public SearchBookingsQueryValidator()
    {
        RuleFor(x => x.To).GreaterThan(x => x.From)
            .When(x => x.From.HasValue && x.To.HasValue)
            .WithMessage("To must be after From.");
    }
}
