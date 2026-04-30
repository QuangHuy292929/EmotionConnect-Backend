using Application.DTOs.Auth;
using Application.DTOs.Friendship;
using Domain.Entities;

namespace Infracstructure.Mappers;

public static class FriendshipMapper
{
    public static FriendshipDto ToDto(this Friendship friendship)
    {
        return new FriendshipDto
        {
            Id = friendship.Id,
            RequesterId = friendship.RequesterId,
            AddresseeId = friendship.AddresseeId,
            Status = friendship.Status.ToString(),
            RequestedAt = friendship.RequestedAt,
            RespondedAt = friendship.RespondedAt,
            CancelledAt = friendship.CancelledAt,
            BlockedAt = friendship.BlockedAt,
            Requester = friendship.Requester != null ? UserMapper.ToSummaryDto(friendship.Requester) : new UserSummaryDto(),
            Addressee = friendship.Addressee != null ? UserMapper.ToSummaryDto(friendship.Addressee) : new UserSummaryDto()
        };
    }

    public static List<FriendshipDto> ToDtoList(this List<Friendship> friendships)
    {
        return friendships.Select(ToDto).ToList();
    }
}