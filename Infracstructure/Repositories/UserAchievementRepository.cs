using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class UserAchievementRepository : IUserAchievementRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserAchievementRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(UserAchievement userAchievement, CancellationToken cancellationToken = default)
    {
        await _dbContext.UserAchievements.AddAsync(userAchievement, cancellationToken);
    }

    public async Task<UserAchievement?> GetByIdAsync(Guid userAchievementId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAchievements
            .Include(x => x.Achievement)
            .FirstOrDefaultAsync(x => x.Id == userAchievementId, cancellationToken);
    }

    public async Task<UserAchievement?> GetByUserAndAchievementAsync(Guid userId, Guid achievementId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAchievements
            .Include(x => x.Achievement)
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.AchievementId == achievementId,
                cancellationToken);
    }

    public async Task<List<UserAchievement>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAchievements
            .Include(x => x.Achievement)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.UnlockedAt)
            .ThenByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);
    }
}
