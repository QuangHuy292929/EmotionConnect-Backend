using Application.DTOs.Friendship;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IFriendshipService
    {
        Task<FriendshipDto> SendRequestAsync(Guid requesterId, Guid addresseeId, CancellationToken cancellationToken = default);
        Task<FriendshipDto> AcceptRequestAsync(Guid friendshipId, Guid currentUserId, CancellationToken cancellationToken = default);
        Task<FriendshipDto> RejectRequestAsync(Guid friendshipId, Guid currentUserId, CancellationToken cancellationToken = default);
        Task<FriendshipDto> CancelRequestAsync(Guid friendshipId, Guid currentUserId, CancellationToken cancellationToken = default);
        Task<FriendshipDto> BlockAsync(Guid friendshipId, Guid currentUserId, CancellationToken cancellationToken = default);
        Task RemoveFriendshipAsync(Guid friendshipId, Guid currentUserId, CancellationToken cancellationToken = default);

        Task<FriendshipDto> GetByIdAsync(Guid friendshipId, Guid currentUserId, CancellationToken cancellationToken = default);
        Task<FriendshipDto?> GetByUsersAsync(Guid currentUserId, Guid otherUserId, CancellationToken cancellationToken = default);
        Task<List<FriendshipDto>> GetIncomingRequestsAsync(Guid currentUserId, CancellationToken cancellationToken = default);
        Task<List<FriendshipDto>> GetOutgoingRequestsAsync(Guid currentUserId, CancellationToken cancellationToken = default);
        Task<List<FriendshipDto>> GetFriendsAsync(Guid currentUserId, CancellationToken cancellationToken = default);

        Task<bool> ExistsBetweenUsersAsync(Guid currentUserId, Guid otherUserId, CancellationToken cancellationToken = default);
    }
}
