using Application.DTOs.AIChat;
using Application.DTOs.Room;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices
{
    public interface IAiChatService
    {
        Task<RoomDto> GetOrCreateAiRoomAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<AiChatResponseDto> SendMessageAsync(SendAiChatMessageRequest request, Guid userId, CancellationToken cancellationToken = default);
    }
}
