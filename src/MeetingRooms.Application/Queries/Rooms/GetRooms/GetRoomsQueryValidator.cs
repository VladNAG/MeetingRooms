using FluentValidation;

namespace MeetingRooms.Application.Queries.Rooms.GetRooms;

public class GetRoomsQueryValidator : AbstractValidator<GetRoomsQuery>
{
    public GetRoomsQueryValidator()
    {
        RuleFor(x => x.MinCapacity).GreaterThan(0).When(x => x.MinCapacity.HasValue);
    }
}
