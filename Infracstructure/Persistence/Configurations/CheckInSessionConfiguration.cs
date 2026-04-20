using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infracstructure.Persistence.Configurations;

public class CheckInSessionConfiguration : IEntityTypeConfiguration<CheckInSession>
{
    public void Configure(EntityTypeBuilder<CheckInSession> builder)
    {
        builder.ToTable("checkin_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CurrentStep)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.InputMode)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.EmotionAnswer)
            .HasMaxLength(2000);

        builder.Property(x => x.IssueAnswer)
            .HasMaxLength(3000);

        builder.Property(x => x.DeepDiveAnswer)
            .HasMaxLength(4000);

        builder.Property(x => x.GeneratedSummary)
            .HasMaxLength(5000);

        builder.Property(x => x.EditedSummary)
            .HasMaxLength(5000);

        builder.Property(x => x.ConfirmedSummary)
            .HasMaxLength(5000);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.EmotionEntry)
            .WithMany()
            .HasForeignKey(x => x.EmotionEntryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => new { x.UserId, x.Status });
        builder.HasIndex(x => new { x.UserId, x.CreatedAt });
    }
}
