using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class CheckInSession : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid? EmotionEntryId { get; set; }

        public CheckInStatus Status { get; set; } = CheckInStatus.Started;
        public CheckInStep CurrentStep { get; set; } = CheckInStep.Step1Emotion;
        public CheckInInputMode InputMode { get; set; } = CheckInInputMode.Text;

        public string? EmotionAnswer { get; set; }
        public string? IssueAnswer { get; set; }
        public string? DeepDiveAnswer { get; set; }

        public string? GeneratedSummary { get; set; }
        public string? EditedSummary { get; set; }
        public string? ConfirmedSummary { get; set; }

        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public User User { get; set; } = null!;
        public EmotionEntry? EmotionEntry { get; set; }
    }
}
