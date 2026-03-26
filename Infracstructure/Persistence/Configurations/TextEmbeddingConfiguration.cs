using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infracstructure.Persistence.Configurations;

public class TextEmbeddingConfiguration : IEntityTypeConfiguration<TextEmbedding>
{
    public void Configure(EntityTypeBuilder<TextEmbedding> builder)
    {
        builder.ToTable("text_embeddings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ModelName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.ModelVersion)
            .HasMaxLength(50);

        builder.Property(x => x.Embedding)
            .HasColumnType("vector(384)")
            .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => x.EmotionEntryId).IsUnique();
        builder.HasIndex(x => x.Embedding)
            .HasMethod("ivfflat")
            .HasOperators("vector_cosine_ops");
    }
}
