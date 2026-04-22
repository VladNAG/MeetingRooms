using FluentValidation.TestHelper;
using MeetingRooms.Application.Commands.Bookings.CancelBooking;

namespace MeetingRooms.UnitTests.Validators;

public class CancelBookingCommandValidatorTests
{
    private readonly CancelBookingCommandValidator _validator = new();

    private static CancelBookingCommand Valid() => new()
    {
        BookingId = Guid.NewGuid(),
        UserId    = Guid.NewGuid(),
        Reason    = "Cancelled by user"
    };

    [Fact]
    public void Valid_Request_Passes()
    {
        _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Reason_NullOrWhitespace_FailsOnReason(string reason)
    {
        var cmd = Valid();
        cmd.Reason = reason;

        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public void BookingId_Empty_FailsOnBookingId()
    {
        var cmd = Valid();
        cmd.BookingId = Guid.Empty;

        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.BookingId);
    }

    [Fact]
    public void UserId_Empty_FailsOnUserId()
    {
        var cmd = Valid();
        cmd.UserId = Guid.Empty;

        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(x => x.UserId);
    }
}
