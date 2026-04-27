using System;
namespace Application.DTOs.CheckIn;

public class CheckInSessionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? EmotionEntryId { get; set; }

    public string Status { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string InputMode { get; set; } = string.Empty;
    public string? CurrentQuestion { get; set; }
    public string? EmotionQuestion { get; set; }
    public string? IssueQuestion { get; set; }
    public string? DeepDiveQuestion { get; set; }
    public string? ReviewQuestion { get; set; }

    public string? EmotionAnswer { get; set; }
    public string? IssueAnswer { get; set; }
    public string? DeepDiveAnswer { get; set; }

    public string? GeneratedSummary { get; set; }
    public string? EditedSummary { get; set; }
    public string? ConfirmedSummary { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}
