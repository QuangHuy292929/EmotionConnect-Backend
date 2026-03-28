using Application.DTOs.Community;
using Application.Interfaces;
using Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommunityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<CommunityDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CommunityDto?> GetByIdAsync(Guid communityId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<CommunityDto>> GetJoinedCommunitiesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task JoinAsync(Guid communityId, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task LeaveAsync(Guid communityId, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
