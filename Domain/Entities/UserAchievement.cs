using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class UserAchievement : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid AchievementId { get; set; }
        public int ProgressValue { get; set; } = 0;
        public bool IsUnlocked { get; set; } = false;
        public DateTime? UnlockedAt { get; set; }

        public User User { get; set; } = null!;
        public Achievement Achievement { get; set; } = null!;
    }
}
