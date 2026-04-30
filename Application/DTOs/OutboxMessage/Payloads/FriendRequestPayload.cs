using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.OutboxMessage.Payloads
{
    public class FriendRequestPayload
    {
        public Guid FriendshipId { get; set; }
        public Guid RequesterId { get; set; }
        public Guid AddresseeId { get; set; }
    }
}
