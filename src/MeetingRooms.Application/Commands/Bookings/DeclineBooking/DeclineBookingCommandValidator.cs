using FluentValidation;

namespace MeetingRooms.Application.Commands.Bookings.DeclineBooking;

public class DeclineBookingCommandValidator : AbstractValidator<DeclineBookingCommand>
{
    public DeclineBookingCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty();
    }
}
