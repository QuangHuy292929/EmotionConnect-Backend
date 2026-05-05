using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.OutboxMessage.Payloads
{
    public class AchievementUnlockedPayload
    {
        public Guid UserId { get; set; }
        public string AchievementCode { get; set; } = string.Empty;
    }
}
