using Domain.Entities;

namespace Application.Interfaces.IRepositories;

public interface IUserAchievementRepository
{
    Task AddAsync(UserAchievement userAchievement, CancellationToken cancellationToken = default);
    Task<UserAchievement?> GetByIdAsync(Guid userAchievementId, CancellationToken cancellationToken = default);
    Task<UserAchievement?> GetByUserAndAchievementAsync(Guid userId, Guid achievementId, CancellationToken cancellationToken = default);
    Task<List<UserAchievement>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
