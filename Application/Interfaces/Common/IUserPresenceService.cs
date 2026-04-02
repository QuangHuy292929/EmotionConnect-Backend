namespace Application.Interfaces.Common;

public interface IUserPresenceService
{
    Task UserConnectedAsync(Guid userId, string connectionId, CancellationToken cancellationToken = default);
    Task UserDisconnectedAsync(Guid userId, string connectionId, CancellationToken cancellationToken = default);
    Task UpdateLastActiveAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsOnlineAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<DateTime?> GetLastActiveAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetActiveConnectionCountAsync(Guid userId, CancellationToken cancellationToken = default);
}
