using Application.DTOs.OutboxMessage;
using Domain.Entities;

namespace Infracstructure.Mappers;

public static class OutboxMessageMapper
{
    public static OutboxMessageDto ToDto(this OutBoxMessage message)
    {
        return new OutboxMessageDto
        {
            Id = message.Id,
            EventType = message.EventType.ToString(),
            AggregateType = message.AggregateType?.ToString(),
            AggregateId = message.AggregateId,
            PayloadJson = message.PayloadJson,
            OccurredAt = message.OccurredAt,
            ProcessedAt = message.ProcessedAt,
            Status = message.Status.ToString(),
            RetryCount = message.RetryCount,
            LastError = message.LastError,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt
        };
    }

    public static List<OutboxMessageDto> ToDtoList(this IEnumerable<OutBoxMessage> messages)
    {
        return messages.Select(ToDto).ToList();
    }
}
