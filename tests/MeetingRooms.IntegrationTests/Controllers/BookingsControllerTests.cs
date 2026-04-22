using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MeetingRooms.Contracts.Responses.Booking;
using MeetingRooms.Domain.Enums;
using MeetingRooms.IntegrationTests.Infrastructure;

namespace MeetingRooms.IntegrationTests.Controllers;

public class BookingsControllerTests(DatabaseFixture fixture) : IntegrationTest(fixture)
{
    private static readonly DateTimeOffset Start = DbSeeder.SlotStart;
    private static readonly DateTimeOffset End = DbSeeder.SlotEnd;

    // ── POST /api/bookings ──────────────────────────────────────────────────

    [Fact]
    public async Task CreateBooking_ValidRequest_Returns201WithDraftStatus()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        Client.WithEmployee(Guid.NewGuid());

        // Act
        var result = await Client.PostAsync<BookingDetailsResponse>("/api/bookings", new
        {
            roomId = room.Id,
            startAt = Start,
            endAt = End,
            purpose = "Standup",
            attendees = new[] { "alice@example.com" }
        });

        // Assert
        result.ShouldBe(HttpStatusCode.Created);
        result.Data!.RoomId.Should().Be(room.Id);
        result.Data.Status.Should().Be("Draft");
        result.Data.Attendees.Should().ContainSingle("alice@example.com");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@domain.com")]
    public async Task CreateBooking_InvalidAttendeeEmail_Returns400(string email)
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        Client.WithEmployee(Guid.NewGuid());

        // Act
        var result = await Client.PostAsync("/api/bookings", new
        {
            roomId = room.Id,
            startAt = Start,
            endAt = End,
            purpose = "Standup",
            attendees = new[] { email }
        });

        // Assert
        result.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBooking_RoomNotFound_Returns404()
    {
        // Arrange
        Client.WithEmployee(Guid.NewGuid());

        // Act
        var result = await Client.PostAsync("/api/bookings", new
        {
            roomId = Guid.NewGuid(),
            startAt = Start,
            endAt = End,
            purpose = "Standup",
            attendees = new[] { "alice@example.com" }
        });

        // Assert
        result.ShouldBe(HttpStatusCode.NotFound);
    }

    // ── POST /api/bookings/{id}/submit ──────────────────────────────────────

    [Fact]
    public async Task SubmitBooking_OwnDraftBooking_Returns204AndStatusIsSubmitted()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var ownerId = Guid.NewGuid();
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, ownerId);
        Client.WithEmployee(ownerId);

        // Act
        var result = await Client.PostAsync($"/api/bookings/{booking.Id}/submit", new { });

        // Assert
        result.ShouldBe(HttpStatusCode.NoContent);

        var get = await Client.GetAsync<BookingDetailsResponse>($"/api/bookings/{booking.Id}");
        get.Data!.Status.Should().Be("Submitted");
        get.Data.History.Should().ContainSingle(h =>
            h.FromStatus == "Draft" && h.ToStatus == "Submitted");
    }

    [Fact]
    public async Task SubmitBooking_ConflictWithExistingConfirmed_Returns422()
    {
        // Arrange — confirmed booking already holds the slot
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid(), BookingStatus.Confirmed);

        var ownerId = Guid.NewGuid();
        var conflicting = await DbSeeder.SeedBookingAsync(db, room.Id, ownerId); // same slot
        Client.WithEmployee(ownerId);

        // Act
        var result = await Client.PostAsync($"/api/bookings/{conflicting.Id}/submit", new { });

        // Assert
        result.ShouldBe(HttpStatusCode.UnprocessableEntity);

        using var verify = CreateDbContext();
        var dbBooking = await verify.BookingRequests.FindAsync(conflicting.Id);
        dbBooking!.Status.Should().Be(BookingStatus.Draft);
    }

    [Fact]
    public async Task SubmitBooking_NotOwner_Returns403()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid());
        Client.WithEmployee(Guid.NewGuid()); // different user

        // Act
        var result = await Client.PostAsync($"/api/bookings/{booking.Id}/submit", new { });

        // Assert
        result.ShouldBe(HttpStatusCode.Forbidden);

        using var verify = CreateDbContext();
        var dbBooking = await verify.BookingRequests.FindAsync(booking.Id);
        dbBooking!.Status.Should().Be(BookingStatus.Draft);
    }

    [Fact]
    public async Task SubmitBooking_AlreadySubmitted_Returns422()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var ownerId = Guid.NewGuid();
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, ownerId, BookingStatus.Submitted);
        Client.WithEmployee(ownerId);

        // Act
        var result = await Client.PostAsync($"/api/bookings/{booking.Id}/submit", new { });

        // Assert
        result.ShouldBe(HttpStatusCode.UnprocessableEntity);

        using var verify = CreateDbContext();
        var dbBooking = await verify.BookingRequests.FindAsync(booking.Id);
        dbBooking!.Status.Should().Be(BookingStatus.Submitted);
    }

    // ── Concurrency: unique index + xmin ────────────────────────────────────

    [Fact]
    public async Task ConfirmBooking_TwoBookingsSameSlot_UniqueIndexPreventsSecondConfirm()
    {
        // Arrange — two submitted bookings that want the same confirmed slot
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var booking1 = await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid(), BookingStatus.Submitted);
        var booking2 = await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid(), BookingStatus.Submitted);

        var adminId = Guid.NewGuid();
        var admin1 = Fixture.CreateClient().WithAdmin(adminId);
        var admin2 = Fixture.CreateClient().WithAdmin(adminId);

        // Act — fire both confirms simultaneously
        var confirm1 = admin1.PostAsync($"/api/bookings/{booking1.Id}/confirm", new { });
        var confirm2 = admin2.PostAsync($"/api/bookings/{booking2.Id}/confirm", new { });
        var results = await Task.WhenAll(confirm1, confirm2);

        // Assert — one gets 204, the other gets 422 (unique index violation)
        var statuses = results.Select(r => r.StatusCode).ToList();
        statuses.Should().ContainSingle(s => s == HttpStatusCode.NoContent);
        statuses.Should().ContainSingle(s => s == HttpStatusCode.UnprocessableEntity);

        using var verify = CreateDbContext();
        var b1 = await verify.BookingRequests.FindAsync(booking1.Id);
        var b2 = await verify.BookingRequests.FindAsync(booking2.Id);
        new[] { b1!.Status, b2!.Status }.Should().ContainSingle(s => s == BookingStatus.Confirmed);
    }

    // ── POST /api/bookings/{id}/confirm ─────────────────────────────────────

    [Fact]
    public async Task ConfirmBooking_AsAdmin_Returns204()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid(), BookingStatus.Submitted);
        Client.WithAdmin(Guid.NewGuid());

        // Act
        var result = await Client.PostAsync($"/api/bookings/{booking.Id}/confirm", new { });

        // Assert
        result.ShouldBe(HttpStatusCode.NoContent);

        using var verify = CreateDbContext();
        var dbBooking = await verify.BookingRequests.FindAsync(booking.Id);
        dbBooking!.Status.Should().Be(BookingStatus.Confirmed);
    }

    [Fact]
    public async Task ConfirmBooking_AsEmployee_Returns403()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid(), BookingStatus.Submitted);
        Client.WithEmployee(Guid.NewGuid());

        // Act
        var result = await Client.PostAsync($"/api/bookings/{booking.Id}/confirm", new { });

        // Assert
        result.ShouldBe(HttpStatusCode.Forbidden);

        using var verify = CreateDbContext();
        var dbBooking = await verify.BookingRequests.FindAsync(booking.Id);
        dbBooking!.Status.Should().Be(BookingStatus.Submitted);
    }

    // ── POST /api/bookings/{id}/decline ─────────────────────────────────────

    [Fact]
    public async Task DeclineBooking_AsAdminWithReason_Returns204()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid(), BookingStatus.Submitted);
        Client.WithAdmin(Guid.NewGuid());

        // Act
        var result = await Client.PostAsync($"/api/bookings/{booking.Id}/decline",
            new { reason = "No available staff" });

        // Assert
        result.ShouldBe(HttpStatusCode.NoContent);

        using var verify = CreateDbContext();
        var dbBooking = await verify.BookingRequests.FindAsync(booking.Id);
        dbBooking!.Status.Should().Be(BookingStatus.Declined);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DeclineBooking_EmptyReason_Returns400(string reason)
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid(), BookingStatus.Submitted);
        Client.WithAdmin(Guid.NewGuid());

        // Act
        var result = await Client.PostAsync($"/api/bookings/{booking.Id}/decline", new { reason });

        // Assert
        result.ShouldBe(HttpStatusCode.BadRequest);
    }

    // ── POST /api/bookings/{id}/cancel ──────────────────────────────────────

    [Fact]
    public async Task CancelBooking_OwnConfirmedBooking_Returns204()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var ownerId = Guid.NewGuid();
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, ownerId, BookingStatus.Confirmed);
        Client.WithEmployee(ownerId);

        // Act
        var result = await Client.PostAsync($"/api/bookings/{booking.Id}/cancel",
            new { reason = "Changed plans" });

        // Assert
        result.ShouldBe(HttpStatusCode.NoContent);

        using var verify = CreateDbContext();
        var dbBooking = await verify.BookingRequests.FindAsync(booking.Id);
        dbBooking!.Status.Should().Be(BookingStatus.Cancelled);
    }

    [Fact]
    public async Task CancelBooking_NotOwner_Returns403()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid(), BookingStatus.Confirmed);
        Client.WithEmployee(Guid.NewGuid()); // different user

        // Act
        var result = await Client.PostAsync($"/api/bookings/{booking.Id}/cancel",
            new { reason = "Changed plans" });

        // Assert
        result.ShouldBe(HttpStatusCode.Forbidden);

        using var verify = CreateDbContext();
        var dbBooking = await verify.BookingRequests.FindAsync(booking.Id);
        dbBooking!.Status.Should().Be(BookingStatus.Confirmed);
    }

    [Fact]
    public async Task CancelBooking_NotConfirmedStatus_Returns422()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var ownerId = Guid.NewGuid();
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, ownerId); // Draft
        Client.WithEmployee(ownerId);

        // Act
        var result = await Client.PostAsync($"/api/bookings/{booking.Id}/cancel",
            new { reason = "Changed plans" });

        // Assert
        result.ShouldBe(HttpStatusCode.UnprocessableEntity);

        using var verify = CreateDbContext();
        var dbBooking = await verify.BookingRequests.FindAsync(booking.Id);
        dbBooking!.Status.Should().Be(BookingStatus.Draft);
    }

    // ── GET /api/bookings/{id} ──────────────────────────────────────────────

    [Fact]
    public async Task GetBooking_ExistingSubmitted_Returns200WithTransitionHistory()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var booking = await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid(), BookingStatus.Submitted);
        Client.WithEmployee(Guid.NewGuid());

        // Act
        var result = await Client.GetAsync<BookingDetailsResponse>($"/api/bookings/{booking.Id}");

        // Assert
        result.ShouldBe(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(booking.Id);
        result.Data.Status.Should().Be("Submitted");
        result.Data.History.Should().ContainSingle(h =>
            h.FromStatus == "Draft" && h.ToStatus == "Submitted");
    }

    [Fact]
    public async Task GetBooking_NotFound_Returns404()
    {
        // Arrange
        Client.WithEmployee(Guid.NewGuid());

        // Act
        var result = await Client.GetAsync<BookingDetailsResponse>($"/api/bookings/{Guid.NewGuid()}");

        // Assert
        result.ShouldBe(HttpStatusCode.NotFound);
    }

    // ── Auth headers ────────────────────────────────────────────────────────

    [Theory]
    [InlineData("/api/bookings")]
    [InlineData("/api/rooms")]
    public async Task PostRequest_WithoutAuthHeaders_Returns403(string url)
    {
        // Arrange — client with no X-User-Id / X-User-Role headers
        var client = Fixture.CreateClient();

        // Act — POST endpoints always access userContext.RequireRole()
        var response = await client.PostAsync(url, JsonContent.Create(new { }));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ── GET /api/bookings ───────────────────────────────────────────────────

    [Fact]
    public async Task SearchBookings_FilterByRoomAndStatus_ReturnsOnlyMatching()
    {
        // Arrange
        using var db = CreateDbContext();
        var room = await DbSeeder.SeedRoomAsync(db);
        var otherRoom = await DbSeeder.SeedRoomAsync(db);
        await DbSeeder.SeedBookingAsync(db, room.Id, Guid.NewGuid(), BookingStatus.Submitted);
        await DbSeeder.SeedBookingAsync(db, otherRoom.Id, Guid.NewGuid(), BookingStatus.Submitted,
            DbSeeder.SlotStart.AddHours(2), DbSeeder.SlotEnd.AddHours(2));
        Client.WithEmployee(Guid.NewGuid());

        // Act
        var result = await Client.GetAsync<List<BookingSummaryResponse>>(
            $"/api/bookings?roomId={room.Id}&status=Submitted");

        // Assert
        result.ShouldBe(HttpStatusCode.OK);
        result.Data!.Should().NotBeEmpty()
            .And.OnlyContain(b => b.RoomId == room.Id && b.Status == "Submitted");
    }
}
