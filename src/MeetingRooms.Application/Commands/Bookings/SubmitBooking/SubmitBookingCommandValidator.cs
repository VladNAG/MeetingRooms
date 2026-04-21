using FluentValidation;

namespace MeetingRooms.Application.Commands.Bookings.SubmitBooking;

public class SubmitBookingCommandValidator : AbstractValidator<SubmitBookingCommand>
{
    public SubmitBookingCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
    }
}
