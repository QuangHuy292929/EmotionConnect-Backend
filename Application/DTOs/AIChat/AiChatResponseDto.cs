using Application.DTOs.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.AIChat
{
    public class AiChatResponseDto
    {
        public Guid RoomId { get; set; }
        public MessageDto UserMessage { get; set; } = null!;
        public MessageDto AssistantMessage { get; set; } = null!;
    }
}
