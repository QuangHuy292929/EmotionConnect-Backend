using Application.DTOs.Community;
using Application.Interfaces;
using Application.Interfaces.IServices;
using Domain.Entities;
using Infracstructure.Mappers;

namespace Infracstructure.Services;

public class CommunityService : ICommunityService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommunityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CommunityDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var communities = await _unitOfWork.CommunityRepository.GetAllAsync(cancellationToken);
        return communities.ToDtoList();
    }

    public async Task<CommunityDto?> GetByIdAsync(Guid communityId, CancellationToken cancellationToken = default)
    {
        var community = await _unitOfWork.CommunityRepository.GetByIdAsync(communityId, cancellationToken);
        return community?.ToDto();
    }

    public async Task<List<CommunityDto>> GetJoinedCommunitiesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var communities = await _unitOfWork.CommunityRepository.GetJoinedCommunitiesAsync(userId, cancellationToken);
        return communities.ToDtoList();
    }

    public async Task JoinAsync(Guid communityId, Guid userId, CancellationToken cancellationToken = default)
    {
        var community = await _unitOfWork.CommunityRepository.GetByIdAsync(communityId, cancellationToken);
        if (community is null)
        {
            throw new KeyNotFoundException("Community not found.");
        }

        var alreadyJoined = await _unitOfWork.CommunityRepository.IsUserJoinedAsync(communityId, userId, cancellationToken);
        if (alreadyJoined)
        {
            return;
        }

        var member = new CommunityMember
        {
            CommunityId = communityId,
            UserId = userId
        };

        await _unitOfWork.CommunityRepository.AddMemberAsync(member, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
    }

    public async Task LeaveAsync(Guid communityId, Guid userId, CancellationToken cancellationToken = default)
    {
        var member = await _unitOfWork.CommunityRepository.GetMemberAsync(communityId, userId, cancellationToken);
        if (member is null)
        {
            return;
        }

        await _unitOfWork.CommunityRepository.RemoveMemberAsync(member);
        await _unitOfWork.SaveChangeAsync(cancellationToken);
    }
}
