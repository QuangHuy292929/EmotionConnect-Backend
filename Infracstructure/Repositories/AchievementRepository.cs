using Application.Interfaces.IRepositories;
using Domain.Entities;
using Infracstructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infracstructure.Repositories;

public class AchievementRepository : IAchievementRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AchievementRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Achievement achievement, CancellationToken cancellationToken = default)
    {
        await _dbContext.Achievements.AddAsync(achievement, cancellationToken);
    }

    public async Task<Achievement?> GetByIdAsync(Guid achievementId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Achievements
            .FirstOrDefaultAsync(x => x.Id == achievementId, cancellationToken);
    }

    public async Task<Achievement?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Achievements
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<List<Achievement>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Achievements
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
