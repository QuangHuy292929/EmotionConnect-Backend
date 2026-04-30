using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Achievement
{
    public class UserAchievementDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AchievementId { get; set; }
        public bool IsUnlocked { get; set; } = false;
        public DateTime? UnlockedAt { get; set; }
        public int ProgressValue { get; set; } = 0;
        public AchievementDto? Achievement { get; set; }
        public double ProgressPercentage { get; set; } = 0;
        public bool IsCompleted { get; set; }
    }
}
