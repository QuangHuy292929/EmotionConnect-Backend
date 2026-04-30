using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities;

public class OutBoxMessage : BaseEntity
{
    public EventType EventType { get; set; }
    public AggregateType? AggregateType { get; set; }
    public Guid? AggregateId { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public OutBoxStatus Status { get; set; }
    public int RetryCount { get; set; } = 0;
    public string? LastError { get; set; }
}
