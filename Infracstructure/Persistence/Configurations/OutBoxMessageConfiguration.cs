using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Persistence.Configurations
{
    public class OutBoxMessageConfiguration : IEntityTypeConfiguration<OutBoxMessage>
    {
        public void Configure(EntityTypeBuilder<OutBoxMessage> builder)
        {
            builder.ToTable("outbox_messages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventType)
                .HasConversion<string>()
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.AggregateType)
                .HasConversion<string>()
                .HasMaxLength(150);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.PayloadJson)
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(x => x.OccurredAt).IsRequired();
            builder.Property(x => x.RetryCount).IsRequired();
            builder.Property(x => x.LastError)
                .HasMaxLength(2000);

            builder.HasIndex(x => new { x.Status, x.OccurredAt });
            builder.HasIndex(x => x.ProcessedAt);
            builder.HasIndex(x => new { x.AggregateType, x.AggregateId });
            builder.HasIndex(x => x.EventType);
        }
    }
}
