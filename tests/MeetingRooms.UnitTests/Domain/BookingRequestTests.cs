using FluentAssertions;
using MeetingRooms.Domain.Entities;
using MeetingRooms.Domain.Enums;
using MeetingRooms.Domain.Exceptions;

namespace MeetingRooms.UnitTests.Domain;

public class BookingRequestTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly Guid RoomId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid AdminId = Guid.NewGuid();

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyPurpose_ThrowsDomainException(string purpose)
    {
        var act = () => BookingRequest.Create(RoomId, UserId,
            new TimeSlot(Base, Base.AddHours(1)), purpose, ["u@x.com"]);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_EmptyAttendees_ThrowsDomainException()
    {
        var act = () => BookingRequest.Create(RoomId, UserId,
            new TimeSlot(Base, Base.AddHours(1)), "Purpose", []);

        act.Should().Throw<DomainException>();
    }

    private static BookingRequest CreateDraft() =>
        BookingRequest.Create(RoomId, UserId, new TimeSlot(Base, Base.AddHours(1)),
            "Team sync", ["user@example.com"]);

    [Fact]
    public void Submit_FromDraft_SetsSubmittedAndAddsTransition()
    {
        var booking = CreateDraft();

        booking.Submit(UserId);

        booking.Status.Should().Be(BookingStatus.Submitted);
        booking.Transitions.Should().ContainSingle(t =>
            t.FromStatus == BookingStatus.Draft && t.ToStatus == BookingStatus.Submitted);
    }

    [Fact]
    public void Submit_FromSubmitted_ThrowsDomainException()
    {
        var booking = CreateDraft();
        booking.Submit(UserId);

        var act = () => booking.Submit(UserId);

        act.Should().Throw<DomainException>().WithMessage("*Cannot submit*");
    }

    [Fact]
    public void Confirm_FromSubmitted_SetsConfirmedAndAddsTransition()
    {
        var booking = CreateDraft();
        booking.Submit(UserId);

        booking.Confirm(AdminId);

        booking.Status.Should().Be(BookingStatus.Confirmed);
        booking.Transitions.Should().HaveCount(2);
    }

    [Fact]
    public void Confirm_FromDraft_ThrowsDomainException()
    {
        var booking = CreateDraft();

        var act = () => booking.Confirm(AdminId);

        act.Should().Throw<DomainException>().WithMessage("*Cannot confirm*");
    }

    [Fact]
    public void Decline_FromSubmitted_SetsDeclinedWithReason()
    {
        var booking = CreateDraft();
        booking.Submit(UserId);

        booking.Decline("Room unavailable", AdminId);

        booking.Status.Should().Be(BookingStatus.Declined);
        booking.Transitions.Should().ContainSingle(t =>
            t.ToStatus == BookingStatus.Declined && t.Reason == "Room unavailable");
    }

    [Fact]
    public void Decline_FromConfirmed_ThrowsDomainException()
    {
        var booking = CreateDraft();
        booking.Submit(UserId);
        booking.Confirm(AdminId);

        var act = () => booking.Decline("reason", AdminId);

        act.Should().Throw<DomainException>().WithMessage("*Cannot decline*");
    }

    [Fact]
    public void Cancel_FromConfirmed_SetsCancelled()
    {
        var booking = CreateDraft();
        booking.Submit(UserId);
        booking.Confirm(AdminId);

        booking.Cancel("Changed plans", UserId);

        booking.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public void Cancel_FromDraft_ThrowsDomainException()
    {
        var booking = CreateDraft();

        var act = () => booking.Cancel("reason", UserId);

        act.Should().Throw<DomainException>().WithMessage("*Cannot cancel*");
    }
}
