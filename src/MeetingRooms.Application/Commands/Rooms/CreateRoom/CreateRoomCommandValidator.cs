using FluentValidation;

namespace MeetingRooms.Application.Commands.Rooms.CreateRoom;

public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("Capacity must be greater than 0.");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required.");
    }
}
