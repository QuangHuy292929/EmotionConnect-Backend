using Application.DTOs.Message;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Mappers;

namespace Infracstructure.Services
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task DeleteAsync(Guid messageId, Guid userId, CancellationToken cancellationToken = default)
        {
            if (messageId == Guid.Empty) throw new BadRequestException($"Message Id is requried.{nameof(messageId)}");

            if (userId == Guid.Empty) throw new BadRequestException($"User Id is requried. {nameof(userId)}");

            var message = await _unitOfWork.MessageRepository.GetByIdAsync(messageId, cancellationToken);
            if (message is null) throw new NotFoundException($"Message with id {messageId} not found");

            if(message.SenderId != userId) throw new ForbiddenException("User is not the sender of the message");

            if (message.DeletedAt.HasValue) return;

            message.DeletedAt = DateTime.UtcNow;
            message.Content = "[Deleted]";
            
            await _unitOfWork.SaveChangeAsync(cancellationToken);
        }

        public async Task<List<MessageDto>> GetRoomMessagesAsync(Guid roomId, Guid userId, CancellationToken cancellationToken = default)
        {
            if (roomId == Guid.Empty) throw new BadRequestException($"Room Id is requried. {nameof(roomId)}");

            var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId, cancellationToken);
            if (room is null) throw new NotFoundException($"Not found any room. {nameof(roomId)}");

            var isMember = await _unitOfWork.RoomRepository.IsUserInRoomAsync(roomId, userId, cancellationToken);
            if (!isMember) throw new ForbiddenException("User is not a member of the room");

            var messages = await _unitOfWork.MessageRepository.GetByRoomIdAsync(roomId, cancellationToken);
            return messages.Select(m => m.ToDto()).ToList();
        }

        public async Task<MessageDto> SendAsync(SendMessageRequest request, Guid senderId, CancellationToken cancellationToken = default)
        {
            if (request is null) throw new BadRequestException($"Request is not allow null here. {nameof(request)}");

            if (senderId == Guid.Empty) throw new BadRequestException($"Sender Id is requried. {nameof(senderId)}");

            if (request.RoomId == Guid.Empty) throw new BadRequestException($"Room Id is requried. {nameof(request.RoomId)}");

            var room = await _unitOfWork.RoomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room is null)
            {
                throw new NotFoundException($"Room with id {request.RoomId} not found");
            }

            var isMember = await _unitOfWork.RoomRepository.IsUserInRoomAsync(request.RoomId, senderId, cancellationToken);
            if (!isMember)
            {
                throw new ForbiddenException("User is not a member of the room");
            }

            if (!Enum.TryParse<MessageType>(request.MessageType, true, out var messageType))
            {
                throw new ConflictException($"MessageType '{request.MessageType}' is invalid. {nameof(request.MessageType)}");
            }

            var message = new Message
            {
                RoomId = request.RoomId,
                SenderId = senderId,
                Content = request.Content,
                MessageType = messageType
            };

            await _unitOfWork.MessageRepository.AddAsync(message, cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken);

            var createdMessage = await _unitOfWork.MessageRepository.GetByIdAsync(message.Id, cancellationToken);
            return (createdMessage ??  message).ToDto();
        }
    }
}
    