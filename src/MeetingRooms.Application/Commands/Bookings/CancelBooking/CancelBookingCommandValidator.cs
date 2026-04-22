using FluentValidation;

namespace MeetingRooms.Application.Commands.Bookings.CancelBooking;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
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
