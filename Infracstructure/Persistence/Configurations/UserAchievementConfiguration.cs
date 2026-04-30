using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Persistence.Configurations
{
    internal class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
    {
        public void Configure(EntityTypeBuilder<UserAchievement> builder)
        {
            builder.ToTable("user_achievements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.AchievementId).IsRequired();
            builder.Property(x => x.ProgressValue).IsRequired();
            builder.Property(x => x.IsUnlocked).IsRequired();

            builder.HasIndex(x => new { x.UserId, x.AchievementId }).IsUnique();
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.AchievementId);
            builder.HasIndex(x => new { x.UserId, x.IsUnlocked });

            builder.HasOne(x => x.User)
                .WithMany(u => u.UserAchievements)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Achievement)
                .WithMany(a => a.UserAchievements)
                .HasForeignKey(x => x.AchievementId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
