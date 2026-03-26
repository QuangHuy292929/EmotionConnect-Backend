using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infracstructure.Persistence.Configurations;

public class CommunityConfiguration : IEntityTypeConfiguration<Community>
{
    private static readonly DateTime SeedTimestamp = new(2026, 3, 26, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Community> builder)
    {
        builder.ToTable("communities");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Slug)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Category)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.Category);

        builder.HasData(
            new Community
            {
                Id = Guid.Parse("a1f3b2cb-0d53-4e8e-9d6d-6c0be9ec4f11"),
                Slug = "career",
                Name = "Career Support",
                Description = "A supportive space for sharing workplace pressure, career uncertainty, and professional burnout.",
                Category = "Career",
                IsActive = true,
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp
            },
            new Community
            {
                Id = Guid.Parse("c97d2e9a-1de4-46fe-9300-2a76092e6d22"),
                Slug = "study",
                Name = "Study Support",
                Description = "A community for academic stress, exam anxiety, learning struggles, and student life challenges.",
                Category = "Study",
                IsActive = true,
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp
            },
            new Community
            {
                Id = Guid.Parse("f45d3de7-9c1f-4f31-bff0-6b67e2f6a733"),
                Slug = "life",
                Name = "Life Support",
                Description = "A place to connect over personal challenges, family matters, loneliness, and everyday emotional burdens.",
                Category = "Life",
                IsActive = true,
                CreatedAt = SeedTimestamp,
                UpdatedAt = SeedTimestamp
            });
    }
}
