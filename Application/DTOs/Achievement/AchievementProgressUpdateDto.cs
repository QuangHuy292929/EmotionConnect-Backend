using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Achievement
{
    public class AchievementProgressUpdateDto
    {
        public string Code { get; set; } = string.Empty;
        public int PreviousProgress { get; set; } = 0;
        public int CurrentProgress { get; set; }
        public int TargetValue { get; set; }
        public bool UnlockedNow { get; set; }
        public DateTime? UnlockedAt { get; set; }
    }
}
