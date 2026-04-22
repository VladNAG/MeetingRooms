using FluentAssertions;
using FluentValidation.TestHelper;
using MeetingRooms.Application.Commands.Bookings.CreateBooking;

namespace MeetingRooms.UnitTests.Validators;

public class CreateBookingCommandValidatorTests
{
    private readonly CreateBookingCommandValidator _validator = new();

    private static readonly DateTimeOffset Base = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);

    private static CreateBookingCommand Valid() => new()
    {
        RoomId   = Guid.NewGuid(),
        UserId   = Guid.NewGuid(),
        StartAt  = Base,
        EndAt    = Base.AddHours(1),
        Purpose  = "Standup",
        Attendees = ["alice@example.com"]
    };

    [Fact]
    public void Valid_Request_Passes()
    {
        _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Purpose_NullOrWhitespace_FailsOnPurpose(string purpose)
    {
        var cmd = Valid();
        cmd.Purpose = purpose;

        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Purpose);
    }

    [Fact]
    public void Attendees_Empty_FailsOnAttendees()
    {
        var cmd = Valid();
        cmd.Attendees = [];

        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Attendees);
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@domain.com")]
    public void Attendee_InvalidEmail_Fails(string email)
    {
        var cmd = Valid();
        cmd.Attendees = [email];

        _validator.TestValidate(cmd).Errors.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(-60)]  // EndAt before StartAt
    [InlineData(0)]    // EndAt equals StartAt
    public void EndAt_NotAfterStartAt_FailsOnEndAt(int offsetMinutes)
    {
        var cmd = Valid();
        cmd.EndAt = cmd.StartAt.AddMinutes(offsetMinutes);

        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.EndAt);
    }

    [Fact]
    public void RoomId_Empty_FailsOnRoomId()
    {
        var cmd = Valid();
        cmd.RoomId = Guid.Empty;

        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.RoomId);
    }
}
