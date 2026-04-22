using FluentValidation;

namespace MeetingRooms.Application.Commands.Bookings.DeclineBooking;

public class DeclineBookingCommandValidator : AbstractValidator<DeclineBookingCommand>
{
    public DeclineBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
        .NotEmpty()
        .WithMessage("BookingId is required.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");
    }
}
