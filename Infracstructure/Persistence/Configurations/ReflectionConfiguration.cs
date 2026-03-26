using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infracstructure.Persistence.Configurations;

public class ReflectionConfiguration : IEntityTypeConfiguration<Reflection>
{
    public void Configure(EntityTypeBuilder<Reflection> builder)
    {
        builder.ToTable("reflections");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.MoodAfter)
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.RoomId, x.CreatedAt });
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.EmotionEntryId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Reflections)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Room)
            .WithMany(x => x.Reflections)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.EmotionEntry)
            .WithMany(x => x.Reflections)
            .HasForeignKey(x => x.EmotionEntryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
