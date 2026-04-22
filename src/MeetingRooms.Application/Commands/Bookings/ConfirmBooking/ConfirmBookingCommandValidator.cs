using FluentValidation;

namespace MeetingRooms.Application.Commands.Bookings.ConfirmBooking;

public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
        .NotEmpty()
        .WithMessage("BookingId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");
    }
}
