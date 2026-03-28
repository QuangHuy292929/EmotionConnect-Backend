using Application.DTOs.Room;
using Domain.Entities;

namespace Infracstructure.Mappers;

public static class RoomMapper
{
    public static RoomDto ToDto(this Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
            CommunityId = room.CommunityId,
            CommunityName = room.Community?.Name,
            Name = room.Name,
            RoomType = room.RoomType.ToString(),
            Status = room.Status.ToString(),
            MaxMembers = room.MaxMembers,
            CurrentMemberCount = room.Members?.Count ?? 0,
            CreatedById = room.CreatedById,
            CreatedAt = room.CreatedAt,
            ClosedAt = room.ClosedAt
        };
    }

    public static List<RoomDto> ToDtoList(this IEnumerable<Room> rooms)
    {
        return rooms.Select(ToDto).ToList();
    }
}
