using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infracstructure.Persistence.Configurations;

public class EmotionScoreConfiguration : IEntityTypeConfiguration<EmotionScore>
{
    public void Configure(EntityTypeBuilder<EmotionScore> builder)
    {
        builder.ToTable("emotion_scores");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EmotionLabel)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Score)
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.EmotionEntryId, x.EmotionLabel }).IsUnique();
    }
}
