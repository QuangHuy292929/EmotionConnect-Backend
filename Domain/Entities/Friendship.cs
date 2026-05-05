using System;
using System.Collections.Generic;
using System.Text;
using Domain.Enums;

namespace Domain.Entities
{
    public class Friendship : BaseEntity
    {
        public Guid RequesterId { get; set; }
        public Guid AddresseeId { get; set; }
        public Guid UserLowId { get; set; }
        public Guid UserHighId { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? BlockedAt { get; set; }

        public User Requester { get; set; } = null!;
        public User Addressee { get; set; } = null!;
    }
}
