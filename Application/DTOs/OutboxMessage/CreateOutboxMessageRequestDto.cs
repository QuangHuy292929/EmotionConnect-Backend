using Domain.Enums;

namespace Application.DTOs.OutboxMessage;

public class CreateOutboxMessageRequestDto
{
    public EventType EventType { get; set; }
    public AggregateType? AggregateType { get; set; }
    public Guid? AggregateId { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime? OccurredAt { get; set; }
}
