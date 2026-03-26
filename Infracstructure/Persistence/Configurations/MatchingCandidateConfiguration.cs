using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infracstructure.Persistence.Configurations;

public class MatchingCandidateConfiguration : IEntityTypeConfiguration<MatchingCandidate>
{
    public void Configure(EntityTypeBuilder<MatchingCandidate> builder)
    {
        builder.ToTable("matching_candidates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MatchType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.SimilarityScore)
            .HasPrecision(6, 5)
            .IsRequired();

        builder.Property(x => x.MatchReason)
            .HasMaxLength(2000);

        builder.Property(x => x.Rank).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.MatchingRequestId, x.Rank }).IsUnique();
        builder.HasIndex(x => x.CandidateUserId);
        builder.HasIndex(x => x.CandidateRoomId);

        builder.HasOne(x => x.MatchingRequest)
            .WithMany(x => x.Candidates)
            .HasForeignKey(x => x.MatchingRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CandidateUser)
            .WithMany(x => x.MatchingCandidates)
            .HasForeignKey(x => x.CandidateUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.CandidateRoom)
            .WithMany(x => x.CandidateMatches)
            .HasForeignKey(x => x.CandidateRoomId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
