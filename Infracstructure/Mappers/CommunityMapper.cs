using Application.DTOs.Community;
using Domain.Entities;

namespace Infracstructure.Mappers;

public static class CommunityMapper
{
    public static CommunityDto ToDto(this Community community)
    {
        return new CommunityDto
        {
            Id = community.Id,
            Slug = community.Slug,
            Name = community.Name,
            Description = community.Description,
            Category = community.Category,
            IsActive = community.IsActive,
            MemberCount = community.Members?.Count ?? 0,
            RoomCount = community.Rooms?.Count ?? 0
        };
    }

    public static List<CommunityDto> ToDtoList(this IEnumerable<Community> communities)
    {
        return communities.Select(ToDto).ToList();
    }
}
