using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Body { get; set; }
        public string? PayloadJson { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public User User { get; set; } = null!;
    }
}
