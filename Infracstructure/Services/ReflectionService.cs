using Application.DTOs.Reflection;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.IServices;
using Domain.Entities;
using Infracstructure.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.Services
{
    public class ReflectionService : IReflectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReflectionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ReflectionDto> CreateAsync(CreateReflectionRequest request, Guid userId, CancellationToken cancellationToken = default)
        {
            if (request == null) throw new BadRequestException($"Request is not allow null here. {nameof(request)}");
            if (request.RoomId == Guid.Empty) throw new BadRequestException($"RoomId is not allow empty here. {nameof(request.RoomId)}");
            if (string.IsNullOrEmpty(request.Content)) throw new BadRequestException($"Content is not allow null or empty here. {nameof(request.Content)}");

            var room = await _unitOfWork.RoomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room == null) throw new NotFoundException($"Room with id {request.RoomId} is not exist.");

            var isMember = await _unitOfWork.RoomRepository.IsUserInRoomAsync(request.RoomId, userId, cancellationToken);
            if (!isMember) throw new BadRequestException($"User with id {userId} is not a member of room with id {request.RoomId}.");

            var reflection = new Reflection
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoomId = request.RoomId,
                EmotionEntryId = request.EmotionEntryId,
                Content = request.Content,
                MoodAfter = request.MoodAfter,
                HelpfulScore = request.HelpfulScore,
            };

            await _unitOfWork.ReflectionRepository.AddAsync(reflection, cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken);

            return reflection.ToDto();
        }

        public async Task<List<ReflectionDto>> GetByRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId, cancellationToken);
            if (room == null) throw new NotFoundException($"Room with id {roomId} is not exist.");

            var reflections = await _unitOfWork.ReflectionRepository.GetByRoomIdAsync(roomId, cancellationToken);
            return reflections.Select(r => r.ToDto()).ToList();
        }

        public async Task<List<ReflectionDto>> GetMyReflectionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var reflections = await _unitOfWork.ReflectionRepository.GetByUserIdAsync(userId, cancellationToken);
            return reflections.Select(r => r.ToDto()).ToList();
        }
    }
}
