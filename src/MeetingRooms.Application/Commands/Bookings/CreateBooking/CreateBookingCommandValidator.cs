using FluentValidation;

namespace MeetingRooms.Application.Commands.Bookings.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.StartAt).NotEmpty();
        RuleFor(x => x.EndAt).NotEmpty().GreaterThan(x => x.StartAt);
        RuleFor(x => x.Purpose).NotEmpty();
        RuleFor(x => x.Attendees).NotEmpty();
        RuleForEach(x => x.Attendees).NotEmpty();
    }
}
