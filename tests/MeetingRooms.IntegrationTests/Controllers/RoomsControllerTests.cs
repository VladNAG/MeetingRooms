using System.Net;
using FluentAssertions;
using MeetingRooms.Contracts.Responses.Room;
using MeetingRooms.IntegrationTests.Infrastructure;

namespace MeetingRooms.IntegrationTests.Controllers;

public class RoomsControllerTests(DatabaseFixture fixture) : IntegrationTest(fixture)
{
    // ── CREATE ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateRoom_AsAdmin_Returns201WithCreatedRoom()
    {
        // Arrange
        Client.WithAdmin(Guid.NewGuid());

        // Act
        var result = await Client.PostAsync<RoomResponse>("/api/rooms", new
        {
            name = "Conference A",
            capacity = 8,
            location = "Floor 2"
        });

        // Assert
        result.ShouldBe(HttpStatusCode.Created);
        result.Data!.Name.Should().Be("Conference A");
        result.Data.Capacity.Should().Be(8);
        result.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateRoom_AsEmployee_Returns403()
    {
        // Arrange
        Client.WithEmployee(Guid.NewGuid());

        // Act
        var result = await Client.PostAsync("/api/rooms", new
        {
            name = "Room B",
            capacity = 5,
            location = "Floor 1"
        });

        // Assert
        result.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("", 5, "Floor 1")]  // empty name
    [InlineData("Room", 0, "Floor 1")]  // zero capacity
    [InlineData("Room", 5, "")]         // empty location
    public async Task CreateRoom_InvalidInput_Returns400(string name, int capacity, string location)
    {
        // Arrange
        Client.WithAdmin(Guid.NewGuid());

        // Act
        var result = await Client.PostAsync("/api/rooms", new { name, capacity, location });

        // Assert
        result.ShouldBe(HttpStatusCode.BadRequest);
    }

    // ── GET ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetRooms_FilterByIsActive_ReturnsOnlyActiveRooms()
    {
        // Arrange
        using var db = CreateDbContext();
        await DbSeeder.SeedRoomAsync(db, isActive: true);
        await DbSeeder.SeedRoomAsync(db, isActive: false);
        Client.WithEmployee(Guid.NewGuid());

        // Act
        var result = await Client.GetAsync<List<RoomResponse>>("/api/rooms?isActive=true");

        // Assert
        result.ShouldBe(HttpStatusCode.OK);
        result.Data!.Should().NotBeEmpty().And.OnlyContain(r => r.IsActive);
    }

    [Fact]
    public async Task GetRooms_FilterByMinCapacity_ReturnsRoomsAboveThreshold()
    {
        // Arrange
        using var db = CreateDbContext();
        await DbSeeder.SeedRoomAsync(db, capacity: 20);
        Client.WithEmployee(Guid.NewGuid());

        // Act
        var result = await Client.GetAsync<List<RoomResponse>>("/api/rooms?minCapacity=15");

        // Assert
        result.ShouldBe(HttpStatusCode.OK);
        result.Data!.Should().NotBeEmpty().And.OnlyContain(r => r.Capacity >= 15);
    }
}
