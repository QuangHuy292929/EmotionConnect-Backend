using Application.DTOs.Achievement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IAchievementService
    {
        Task<List<AchievementDto>> GetActiveAchievementsAsync(CancellationToken cancellationToken = default);
        Task<List<UserAchievementDto>> GetMyAchievementsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserAchievementDto?> GetUserAchievementAsync(Guid userId, Guid achievementId, CancellationToken cancellationToken = default);
        Task<UserAchievementDto?> GetUserAchievementByCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default);

        Task<UserAchievementDto> InitializeProgressAsync(Guid userId, Guid achievementId, CancellationToken cancellationToken = default);
        Task<AchievementProgressUpdateDto> IncrementProgressAsync(Guid userId, string achievementCode, int amount = 1, CancellationToken cancellationToken = default);
        Task<AchievementProgressUpdateDto> SetProgressAsync(Guid userId, string achievementCode, int progressValue, CancellationToken cancellationToken = default);
        Task<UserAchievementDto> UnlockAsync(Guid userId, string achievementCode, CancellationToken cancellationToken = default);

        Task<AchievementProgressUpdateDto> ProcessEventAsync(Guid userId, string achievementCode, int amount = 1, CancellationToken cancellationToken = default);
    }
}
