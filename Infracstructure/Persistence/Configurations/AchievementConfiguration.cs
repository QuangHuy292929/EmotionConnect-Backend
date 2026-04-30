using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Persistence.Configurations
{
    public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.ToTable("achievements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Category)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.IconUrl).HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1000);

            builder.Property(x => x.TargetValue).IsRequired();
            builder.Property(x => x.IsActive).IsRequired();

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.Category);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
