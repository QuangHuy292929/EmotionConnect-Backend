using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.IRepositories;

public interface IFriendshipRepository
{
    Task AddAsync(Friendship friendship, CancellationToken cancellationToken = default);
    Task RemoveAsync(Friendship friendship, CancellationToken cancellationToken = default);
    Task<Friendship?> GetByIdAsync(Guid friendshipId, CancellationToken cancellationToken = default);
    Task<Friendship?> GetByUsersAsync(Guid userAId, Guid userBId, CancellationToken cancellationToken = default);
    Task<bool> ExistsBetweenUsersAsync(Guid userAId, Guid userBId, CancellationToken cancellationToken = default);
    Task<List<Friendship>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Friendship>> GetIncomingByStatusAsync(Guid addresseeId, FriendshipStatus status, CancellationToken cancellationToken = default);
    Task<List<Friendship>> GetOutgoingByStatusAsync(Guid requesterId, FriendshipStatus status, CancellationToken cancellationToken = default);
}
