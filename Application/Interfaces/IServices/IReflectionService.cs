using System;
using System.Collections.Generic;
using System.Text;
using Application.DTOs.Reflection;

namespace Application.Interfaces.IServices
{
    public interface IReflectionService
    {
        Task<ReflectionDto> CreateAsync(CreateReflectionRequest request, Guid userId, CancellationToken cancellationToken = default);
        Task<List<ReflectionDto>> GetByRoomAsync(Guid roomId, CancellationToken cancellationToken = default);
        Task<List<ReflectionDto>> GetMyReflectionsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
