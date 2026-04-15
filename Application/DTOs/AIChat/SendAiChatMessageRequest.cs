using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.AIChat;

public class SendAiChatMessageRequest
{
    public Guid RoomId { get; set; }
    public string Content { get; set; } = string.Empty;
}
