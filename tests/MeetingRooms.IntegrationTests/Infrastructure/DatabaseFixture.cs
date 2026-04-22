using MeetingRooms.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace MeetingRooms.IntegrationTests.Infrastructure;

public class DatabaseFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("postgres:16-alpine").Build();

    async Task IAsyncLifetime.InitializeAsync()
    {
        await _postgres.StartAsync();

        using var scope = Services.CreateScope();
        await scope.ServiceProvider
            .GetRequiredService<MeetingRoomsDbContext>()
            .Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgres.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<MeetingRoomsDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<MeetingRoomsDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString()));
        });
    }

    public MeetingRoomsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MeetingRoomsDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;
        return new MeetingRoomsDbContext(options);
    }

    public async Task CleanUpAsync()
    {
        using var db = CreateDbContext();
        await db.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE status_transitions, booking_requests, rooms CASCADE");
    }
}
