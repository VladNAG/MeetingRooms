using FluentValidation;

namespace MeetingRooms.Application.Commands.Bookings.SubmitBooking;

public class SubmitBookingCommandValidator : AbstractValidator<SubmitBookingCommand>
{
    public SubmitBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
        .NotEmpty()
        .WithMessage("BookingId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");
    }
}
