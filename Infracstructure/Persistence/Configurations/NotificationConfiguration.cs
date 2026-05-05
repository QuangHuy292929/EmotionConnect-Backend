using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infracstructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Body)
                .HasMaxLength(1000);

            builder.Property(x => x.PayloadJson).HasColumnType("jsonb");
            builder.Property(x => x.IsRead).IsRequired();
            builder.Property(x => x.UserId).IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Type);
            builder.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });

            builder.HasOne(x => x.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
