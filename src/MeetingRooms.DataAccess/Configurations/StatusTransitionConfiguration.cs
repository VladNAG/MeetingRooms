using MeetingRooms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeetingRooms.DataAccess.Configurations;

public class StatusTransitionConfiguration : IEntityTypeConfiguration<StatusTransition>
{
    public void Configure(EntityTypeBuilder<StatusTransition> builder)
    {
        builder.ToTable("status_transitions");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.BookingRequestId).HasColumnName("booking_request_id");
        builder.Property(t => t.FromStatus).HasColumnName("from_status").HasConversion<string>();
        builder.Property(t => t.ToStatus).HasColumnName("to_status").HasConversion<string>();
        builder.Property(t => t.ChangedByUserId).HasColumnName("changed_by_user_id");
        builder.Property(t => t.ChangedAt).HasColumnName("changed_at");
        builder.Property(t => t.Reason).HasColumnName("reason");
    }
}
