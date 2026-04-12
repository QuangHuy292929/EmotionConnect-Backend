using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.Message;

namespace Application.Interfaces.IServices
{
    public interface IMessageService
    {
        Task<MessageDto> SendAsync(SendMessageRequest request, Guid senderId, CancellationToken cancellationToken = default);
        Task<List<MessageDto>> GetRoomMessagesAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default);
    }
}
