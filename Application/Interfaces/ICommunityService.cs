using Application.DTOs.Community;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICommunityService
    {
        Task<IReadOnlyList<CommunityDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CommunityDto?> GetByIdAsync(Guid communityId, CancellationToken cancellationToken = default);
        Task JoinAsync(Guid communityId, Guid userId, CancellationToken cancellationToken = default);
        Task LeaveAsync(Guid communityId, Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CommunityDto>> GetJoinedCommunitiesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<CommunityDto> CreateCommunityAsync(CreateCommunityRequest createCommunityRequest, CancellationToken cancellation = default);
    }
}
