using Application.DTOs.Message;
using Application.Interfaces;
using Application.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task DeleteAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<MessageDto>> GetRoomMessagesAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<MessageDto> SendAsync(SendMessageRequest request, Guid senderId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
