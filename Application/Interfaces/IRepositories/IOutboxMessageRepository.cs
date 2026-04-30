using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.IRepositories;

public interface IOutboxMessageRepository
{
    Task AddAsync(OutBoxMessage message, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<OutBoxMessage> messages, CancellationToken cancellationToken = default);
    Task<OutBoxMessage?> GetByIdAsync(Guid outboxMessageId, CancellationToken cancellationToken = default);
    Task<List<OutBoxMessage>> GetByStatusAsync(OutBoxStatus status, int take, CancellationToken cancellationToken = default);
}
