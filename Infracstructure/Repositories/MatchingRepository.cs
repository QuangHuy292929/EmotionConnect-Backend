using Application.DTOs.Matching;
using Application.Interfaces.IRepositories;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infracstructure.Repositories;

public class MatchingRepository : IMatchingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MatchingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRequestAsync(MatchingRequest request, CancellationToken cancellationToken = default)
    {
        await _dbContext.MatchingRequests.AddAsync(request, cancellationToken);
    }

    public async Task AddCandidatesAsync(IEnumerable<MatchingCandidate> candidates, CancellationToken cancellationToken = default)
    {
        await _dbContext.MatchingCandidates.AddRangeAsync(candidates, cancellationToken);
    }

    public async Task<MatchingRequest?> GetRequestByIdAsync(Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MatchingRequests
            .Include(x => x.Candidates)
            .FirstOrDefaultAsync(x => x.Id == matchingRequestId, cancellationToken);
    }

    public async Task<MatchingRequest?> GetLatestRequestByEmotionEntryIdAsync(Guid emotionEntryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MatchingRequests
            .Include(x => x.Candidates)
            .Where(x => x.EmotionEntryId == emotionEntryId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MatchingRequest?> GetRequestByIdForUserAsync(Guid matchingRequestId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MatchingRequests
            .Include(x => x.Candidates)
            .Include(x => x.AssignedRoom)
            .FirstOrDefaultAsync(
                x => x.Id == matchingRequestId && x.UserId == userId,
                cancellationToken);
    }

    public async Task<List<MatchingCandidate>> GetCandidatesAsync(Guid matchingRequestId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MatchingCandidates
            .Include(x => x.CandidateUser)
            .Include(x => x.CandidateRoom)
            .Where(x => x.MatchingRequestId == matchingRequestId)
            .OrderBy(x => x.Rank)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MatchingCandidateSeed>> FindSimilarUsersAsync(Guid emotionEntryId, Guid userId, CancellationToken cancellationToken = default)
    {
        FormattableString query = $@"
            WITH source_embedding AS (
                SELECT ""Embedding""
                FROM text_embeddings
                WHERE ""EmotionEntryId"" = {emotionEntryId}
            ),
            ranked_candidates AS (
                SELECT DISTINCT ON (candidate_entry.""UserId"")
                    candidate_entry.""UserId"" AS ""CandidateUserId"",
                    CAST(1 - (source_embedding.""Embedding"" <=> candidate_embedding.""Embedding"") AS numeric(6,5)) AS ""SimilarityScore"",
                    'Similar emotional context.' AS ""MatchReason""
                FROM source_embedding
                JOIN text_embeddings candidate_embedding
                    ON candidate_embedding.""EmotionEntryId"" <> {emotionEntryId}
                JOIN emotion_entries candidate_entry
                    ON candidate_entry.""Id"" = candidate_embedding.""EmotionEntryId""
                WHERE candidate_entry.""UserId"" <> {userId}
                ORDER BY candidate_entry.""UserId"", source_embedding.""Embedding"" <=> candidate_embedding.""Embedding""
            )
            SELECT ""CandidateUserId"", ""SimilarityScore"", ""MatchReason""
            FROM ranked_candidates
            ORDER BY ""SimilarityScore"" DESC
            LIMIT 10";

        return await _dbContext.Set<MatchingCandidateSeed>()
            .FromSqlInterpolated(query)
            .ToListAsync(cancellationToken);

    }

    public async Task<List<WaitingRoomMatchDto>> FindEligibleWaitingRoomsAsync(
        Guid matchingRequestId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var requestExists = await _dbContext.MatchingRequests
            .AsNoTracking()
            .AnyAsync(
                x => x.Id == matchingRequestId && x.UserId == userId,
                cancellationToken);

        if (!requestExists)
        {
            return [];
        }

        var connection = _dbContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                WITH candidate_scores AS (
                    SELECT
                        mc."CandidateUserId",
                        mc."SimilarityScore"
                    FROM matching_candidates mc
                    WHERE mc."MatchingRequestId" = @matchingRequestId
                      AND mc."CandidateUserId" IS NOT NULL
                ),
                waiting_room_members AS (
                    SELECT
                        r."Id" AS "RoomId",
                        r."CreatedAt",
                        r."MaxMembers",
                        rm."UserId"
                    FROM rooms r
                    JOIN room_members rm
                        ON rm."RoomId" = r."Id"
                    WHERE r."RoomType" = @roomType
                      AND r."Status" = @roomStatus
                      AND rm."MemberState" = @memberState
                      AND NOT EXISTS (
                          SELECT 1
                          FROM room_members existing_member
                          WHERE existing_member."RoomId" = r."Id"
                            AND existing_member."UserId" = @userId
                            AND existing_member."MemberState" = @memberState
                      )
                ),
                room_member_counts AS (
                    SELECT
                        wrm."RoomId",
                        COUNT(DISTINCT wrm."UserId")::int AS "MemberCount",
                        MAX(wrm."MaxMembers") AS "MaxMembers",
                        MAX(wrm."CreatedAt") AS "CreatedAt"
                    FROM waiting_room_members wrm
                    GROUP BY wrm."RoomId"
                ),
                eligible_scores AS (
                    SELECT
                        wrm."RoomId",
                        rmc."CreatedAt",
                        rmc."MemberCount",
                        rmc."MaxMembers",
                        MAX(cs."SimilarityScore") AS "BestSimilarityScore"
                    FROM waiting_room_members wrm
                    JOIN candidate_scores cs
                        ON cs."CandidateUserId" = wrm."UserId"
                    JOIN room_member_counts rmc
                        ON rmc."RoomId" = wrm."RoomId"
                    GROUP BY wrm."RoomId", rmc."CreatedAt", rmc."MemberCount", rmc."MaxMembers"
                )
                SELECT
                    es."RoomId",
                    es."BestSimilarityScore",
                    es."MemberCount",
                    es."CreatedAt"
                FROM eligible_scores es
                WHERE es."MemberCount" < es."MaxMembers"
                ORDER BY es."BestSimilarityScore" DESC, es."CreatedAt" ASC
                """;

            var matchingRequestIdParameter = command.CreateParameter();
            matchingRequestIdParameter.ParameterName = "@matchingRequestId";
            matchingRequestIdParameter.Value = matchingRequestId;
            command.Parameters.Add(matchingRequestIdParameter);

            var userIdParameter = command.CreateParameter();
            userIdParameter.ParameterName = "@userId";
            userIdParameter.Value = userId;
            command.Parameters.Add(userIdParameter);

            var roomTypeParameter = command.CreateParameter();
            roomTypeParameter.ParameterName = "@roomType";
            roomTypeParameter.Value = RoomType.Matching.ToString();
            command.Parameters.Add(roomTypeParameter);

            var roomStatusParameter = command.CreateParameter();
            roomStatusParameter.ParameterName = "@roomStatus";
            roomStatusParameter.Value = RoomStatus.Waiting.ToString();
            command.Parameters.Add(roomStatusParameter);

            var memberStateParameter = command.CreateParameter();
            memberStateParameter.ParameterName = "@memberState";
            memberStateParameter.Value = RoomMemberState.Active.ToString();
            command.Parameters.Add(memberStateParameter);

            var results = new List<WaitingRoomMatchDto>();

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(new WaitingRoomMatchDto
                {
                    RoomId = reader.GetGuid(0),
                    BestSimilarityScore = reader.GetDecimal(1),
                    MemberCount = reader.GetInt32(2),
                    CreatedAt = reader.GetDateTime(3)
                });
            }

            return results;
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    public async Task<Room?> LockWaitingRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var roomType = RoomType.Matching.ToString();
        var roomStatus = RoomStatus.Waiting.ToString();

        return await _dbContext.Rooms
            .FromSqlInterpolated($@"
                SELECT *
                FROM rooms
                WHERE ""Id"" = {roomId}
                  AND ""RoomType"" = {roomType}
                  AND ""Status"" = {roomStatus}
                FOR UPDATE")
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsUserInAnyOpenMatchingRoomAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var openStatuses = new[]
        {
            RoomStatus.Waiting,
            RoomStatus.Ready,
            RoomStatus.Active
        };

        return await _dbContext.RoomMembers.AnyAsync(
            x => x.UserId == userId &&
                 x.MemberState == RoomMemberState.Active &&
                 x.Room.RoomType == RoomType.Matching &&
                 openStatuses.Contains(x.Room.Status),
            cancellationToken);
    }
}
