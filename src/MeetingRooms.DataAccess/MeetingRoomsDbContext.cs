using MeetingRooms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRooms.DataAccess;

public class MeetingRoomsDbContext(DbContextOptions<MeetingRoomsDbContext> options) : DbContext(options)
{
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<BookingRequest> BookingRequests => Set<BookingRequest>();
    public DbSet<StatusTransition> StatusTransitions => Set<StatusTransition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeetingRoomsDbContext).Assembly);
    }
}
