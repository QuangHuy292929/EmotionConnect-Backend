using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infracstructure.Persistence.Configurations;

public class EmotionEntryConfiguration : IEntityTypeConfiguration<EmotionEntry>
{
    public void Configure(EntityTypeBuilder<EmotionEntry> builder)
    {
        builder.ToTable("emotion_entries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SourceType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.RawText)
            .IsRequired();

        builder.Property(x => x.TopEmotion)
            .HasMaxLength(100);

        builder.Property(x => x.TopEmotionScore)
            .HasPrecision(5, 4);

        builder.Property(x => x.SentimentScore)
            .HasPrecision(5, 4);

        builder.Property(x => x.LanguageCode)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.CreatedAt });
        builder.HasIndex(x => x.CommunityId);
        builder.HasIndex(x => x.RoomId);
        builder.HasIndex(x => x.TopEmotion);

        builder.HasOne(x => x.User)
            .WithMany(x => x.EmotionEntries)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Community)
            .WithMany(x => x.EmotionEntries)
            .HasForeignKey(x => x.CommunityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Room)
            .WithMany(x => x.EmotionEntries)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Scores)
            .WithOne(x => x.EmotionEntry)
            .HasForeignKey(x => x.EmotionEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Embedding)
            .WithOne(x => x.EmotionEntry)
            .HasForeignKey<TextEmbedding>(x => x.EmotionEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
