using FluentAssertions;
using MeetingRooms.Domain.Enums;
using MeetingRooms.Domain.Exceptions;
using MeetingRooms.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MeetingRooms.IntegrationTests.Concurrency;

public class XminConcurrencyTests(DatabaseFixture fixture) : IntegrationTest(fixture)
{
    [Fact]
    public async Task Confirm_TwoContextsLoadSameRow_SecondThrowsDomainException()
    {
        // Arrange — seed one Submitted booking
        await using var seed = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(seed);
        var booking = await DbSeeder.SeedBookingAsync(seed, room.Id, Guid.NewGuid(),
                          BookingStatus.Submitted);

        // Two separate contexts load the same row — same xmin in both
        await using var db1 = CreateDbContext();
        await using var db2 = CreateDbContext();

        var b1 = await db1.BookingRequests.FirstAsync(b => b.Id == booking.Id);
        var b2 = await db2.BookingRequests.FirstAsync(b => b.Id == booking.Id);

        // Act — first context wins, commits, xmin changes in DB
        b1.Confirm(Guid.NewGuid());
        await db1.SaveChangesAsync();

        // Second context has stale xmin — must throw
        b2.Confirm(Guid.NewGuid());
        var act = () => db2.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*modified by another request*");
    }

    [Fact]
    public async Task Cancel_TwoContextsLoadSameRow_SecondThrowsDomainException()
    {
        // Arrange
        await using var seed = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(seed);
        var ownerId = Guid.NewGuid();
        var booking = await DbSeeder.SeedBookingAsync(seed, room.Id, ownerId,
                          BookingStatus.Confirmed);

        // Two separate contexts load the same row — same xmin in both
        await using var db1 = CreateDbContext();
        await using var db2 = CreateDbContext();

        var b1 = await db1.BookingRequests.FirstAsync(b => b.Id == booking.Id);
        var b2 = await db2.BookingRequests.FirstAsync(b => b.Id == booking.Id);

        // Act — first cancel commits, xmin rotates
        b1.Cancel("First request", ownerId);
        await db1.SaveChangesAsync();

        b2.Cancel("Second request", ownerId);
        var act = () => db2.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*modified by another request*");
    }
}
