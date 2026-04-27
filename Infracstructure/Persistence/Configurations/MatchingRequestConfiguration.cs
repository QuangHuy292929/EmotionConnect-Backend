using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infracstructure.Persistence.Configurations;

public class MatchingRequestConfiguration : IEntityTypeConfiguration<MatchingRequest>
{
    public void Configure(EntityTypeBuilder<MatchingRequest> builder)
    {
        builder.ToTable("matching_requests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequestStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.CreatedAt });
        builder.HasIndex(x => x.RequestStatus);
        builder.HasIndex(x => x.AssignedRoomId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.MatchingRequests)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.EmotionEntry)
            .WithMany(x => x.MatchingRequests)
            .HasForeignKey(x => x.EmotionEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AssignedRoom)
            .WithMany(x => x.MatchingRequests)
            .HasForeignKey(x => x.AssignedRoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
