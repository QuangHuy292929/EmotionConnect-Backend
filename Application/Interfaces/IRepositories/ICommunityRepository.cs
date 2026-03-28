using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface ICommunityRepository
    {
        Task<List<Community>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Community?> GetByIdAsync(Guid communityId, CancellationToken cancellationToken = default);
        Task<Community?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<bool> IsUserJoinedAsync(Guid communityId, Guid userId, CancellationToken cancellationToken = default);
        Task AddMemberAsync(CommunityMember member, CancellationToken cancellationToken = default);
        Task RemoveMemberAsync(CommunityMember member);
        Task<List<Community>> GetJoinedCommunitiesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<CommunityMember?> GetMemberAsync(Guid communityId, Guid userId, CancellationToken cancellationToken = default);
    }
}
