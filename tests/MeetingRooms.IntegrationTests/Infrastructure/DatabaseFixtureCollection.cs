namespace MeetingRooms.IntegrationTests.Infrastructure;

[CollectionDefinition(nameof(DatabaseFixtureCollection))]
public class DatabaseFixtureCollection : ICollectionFixture<DatabaseFixture> { }
