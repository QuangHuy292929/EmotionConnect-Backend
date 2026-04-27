using Application.DTOs.Room;
using Domain.Entities;
using System.Linq; // nhớ cái này

namespace Infracstructure.Mappers;

public static class RoomMapper
{
    // ===== ROOM =====
    public static RoomDto ToDto(this Room room)
    {
        return new RoomDto
        {
            Id = room.Id,
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

    // ===== ROOM MEMBER =====
    public static RoomMemberDto ToDto(this RoomMember entity)
    {
        return new RoomMemberDto
        {
            UserId = entity.UserId,
            Username = entity.User?.Username ?? string.Empty,
            DisplayName = entity.User?.DisplayName,
            AvatarUrl = entity.User?.AvatarUrl
        };
    }

    public static List<RoomMemberDto> ToDtoList(this IEnumerable<RoomMember> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }
}
