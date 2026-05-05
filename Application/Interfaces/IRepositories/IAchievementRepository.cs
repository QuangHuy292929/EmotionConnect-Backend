using Domain.Entities;

namespace Application.Interfaces.IRepositories;

public interface IAchievementRepository
{
    Task AddAsync(Achievement achievement, CancellationToken cancellationToken = default);
    Task<Achievement?> GetByIdAsync(Guid achievementId, CancellationToken cancellationToken = default);
    Task<Achievement?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<Achievement>> GetActiveAsync(CancellationToken cancellationToken = default);
}
