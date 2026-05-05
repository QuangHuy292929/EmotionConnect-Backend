using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Persistence.Configurations
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.ToTable("friendships");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.RequesterId).IsRequired();
            builder.Property(x => x.AddresseeId).IsRequired();
            builder.Property(x => x.UserLowId).IsRequired();
            builder.Property(x => x.UserHighId).IsRequired();
            builder.Property(x => x.RequestedAt).IsRequired();

            builder.HasIndex(x => new { x.UserLowId, x.UserHighId }).IsUnique();
            builder.HasIndex(x => x.AddresseeId);
            builder.HasIndex(x => x.RequesterId);
            builder.HasIndex(x => x.Status);

            builder.HasOne(x => x.Requester)
                .WithMany(u => u.FriendshipsInitiated)
                .HasForeignKey(x => x.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Addressee)
                .WithMany(u => u.FriendshipsReceived)
                .HasForeignKey(x => x.AddresseeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
