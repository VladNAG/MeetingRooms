using MeetingRooms.Domain.Entities;
using MeetingRooms.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainException("The booking was modified by another request. Please retry.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            throw new DomainException("The time slot is already confirmed for this room.");
        }
    }
}
