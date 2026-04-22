using MeetingRooms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingRooms.DataAccess.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("rooms");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");
        builder.Property(r => r.Name).HasColumnName("name").IsRequired();
        builder.Property(r => r.Capacity).HasColumnName("capacity");
        builder.Property(r => r.Location).HasColumnName("location").IsRequired();
        builder.Property(r => r.IsActive).HasColumnName("is_active");

        builder.Property<uint>("xmin")
             .IsRowVersion();
    }
}
