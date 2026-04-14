using Application.DTOs.Message;
using Domain.Entities;

namespace Infracstructure.Mappers;

public static class MessageMapper
{
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            RoomId = message.RoomId,
            SenderId = message.SenderId,
            SenderUsername = message.Sender?.Username,
            MessageType = message.MessageType.ToString(),
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            EditedAt = message.EditedAt,
            DeletedAt = message.DeletedAt
        };
    }

    public static List<MessageDto> ToDtoList(this IEnumerable<Message> messages)
    {
        return messages.Select(ToDto).ToList();
    }
}
