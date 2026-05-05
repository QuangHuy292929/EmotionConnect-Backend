namespace Application.DTOs.OutboxMessage;

public class OutboxMessageDto
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? AggregateType { get; set; }
    public Guid? AggregateId { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public string? LastError { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
