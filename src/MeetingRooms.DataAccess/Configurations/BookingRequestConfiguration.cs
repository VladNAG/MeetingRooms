using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingRooms.DataAccess.Configurations;

public class BookingRequestConfiguration : IEntityTypeConfiguration<BookingRequest>
{
    public void Configure(EntityTypeBuilder<BookingRequest> builder)
    {
        builder.ToTable("booking_requests");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasColumnName("id");
        builder.Property(b => b.RoomId).HasColumnName("room_id");
        builder.Property(b => b.RequestedByUserId).HasColumnName("requested_by_user_id");
        builder.Property(b => b.Purpose).HasColumnName("purpose").IsRequired();
        builder.Property(b => b.Status)
            .HasColumnName("status")
            .HasConversion<string>();

        builder.Property(b => b.Attendees)
            .HasColumnName("attendees")
            .HasColumnType("jsonb");

        builder.OwnsOne(b => b.TimeSlot, ts =>
        {
            ts.Property(t => t.StartAt).HasColumnName("start_at");
            ts.Property(t => t.EndAt).HasColumnName("end_at");
        });

        builder.Property<uint>("xmin")
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.Navigation(b => b.Transitions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(b => b.Transitions)
            .WithOne()
            .HasForeignKey(t => t.BookingRequestId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
