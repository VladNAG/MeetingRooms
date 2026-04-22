using FluentAssertions;
using MeetingRooms.Domain.Entities;
using MeetingRooms.Domain.Exceptions;

namespace MeetingRooms.UnitTests.Domain;

public class TimeSlotTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WhenEndBeforeStart_ThrowsDomainException()
    {
        var act = () => new TimeSlot(Base.AddHours(2), Base);
        act.Should().Throw<DomainException>().WithMessage("*EndAt must be after StartAt*");
    }

    [Fact]
    public void Create_WhenEndEqualsStart_ThrowsDomainException()
    {
        var act = () => new TimeSlot(Base, Base);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WhenDurationExceeds4Hours_ThrowsDomainException()
    {
        var act = () => new TimeSlot(Base, Base.AddHours(4).AddMinutes(1));
        act.Should().Throw<DomainException>().WithMessage("*cannot exceed 4 hours*");
    }

    [Fact]
    public void Create_WhenDurationExactly4Hours_Succeeds()
    {
        var slot = new TimeSlot(Base, Base.AddHours(4));
        slot.StartAt.Should().Be(Base);
        slot.EndAt.Should().Be(Base.AddHours(4));
    }

    [Fact]
    public void Create_ValidSlot_SetsProperties()
    {
        var slot = new TimeSlot(Base, Base.AddHours(1));
        slot.StartAt.Should().Be(Base);
        slot.EndAt.Should().Be(Base.AddHours(1));
    }
}
