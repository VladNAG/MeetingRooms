using MeetingRooms.DataAccess;

namespace MeetingRooms.IntegrationTests.Infrastructure;

[Collection(nameof(DatabaseFixtureCollection))]
public abstract class IntegrationTest(DatabaseFixture fixture) : IAsyncLifetime
{
    protected DatabaseFixture Fixture { get; } = fixture;
    protected HttpClient Client { get; } = fixture.CreateClient();

    public Task InitializeAsync() => Fixture.CleanUpAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    protected MeetingRoomsDbContext CreateDbContext() => Fixture.CreateDbContext();
}
