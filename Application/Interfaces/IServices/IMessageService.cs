using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.Message;

namespace Application.Interfaces.IServices
{
    public interface IMessageService
    {
        Task<MessageDto> SendAsync(SendMessageRequest request, Guid senderId, CancellationToken cancellationToken = default);
        Task<MessageDto?> GetByIdAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default);
        Task<PagedResult<MessageDto>> GetRoomMessagesAsync(Guid roomId, Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<MessageDto>> SearchRoomMessagesAsync(Guid roomId, Guid userId, string keyword, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<MessageDto> EditAsync(Guid messageId, EditMessageRequest request, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default);

    }
}
