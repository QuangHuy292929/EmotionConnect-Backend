using Application.DTOs.Matching;
using Domain.Entities;

namespace Infracstructure.Mappers;

public static class MatchingMapper
{
    public static MatchingCandidateDto ToDto(this MatchingCandidate candidate)
    {
        return new MatchingCandidateDto
        {
            Id = candidate.Id,
            CandidateUserId = candidate.CandidateUserId,
            CandidateRoomId = candidate.CandidateRoomId,
            MatchType = candidate.MatchType.ToString(),
            SimilarityScore = candidate.SimilarityScore,
            MatchReason = candidate.MatchReason,
            Rank = candidate.Rank
        };
    }

    public static MatchingResultDto ToResultDto(this MatchingRequest request)
    {
        return new MatchingResultDto
        {
            MatchingRequestId = request.Id,
            EmotionEntryId = request.EmotionEntryId,
            Candidates = request.Candidates?.Select(ToDto).OrderBy(x => x.Rank).ToList() ?? new List<MatchingCandidateDto>()
        };
    }

    public static List<MatchingCandidateDto> ToDtoList(this IEnumerable<MatchingCandidate> candidates)
    {
        return candidates.Select(ToDto).OrderBy(x => x.Rank).ToList();
    }
}
