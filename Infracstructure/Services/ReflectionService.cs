using Application.DTOs.Reflection;
using Application.Interfaces;
using Application.Interfaces.IServices;
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
        public Task<ReflectionDto> CreateAsync(CreateReflectionRequest request, Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<ReflectionDto>> GetByRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<ReflectionDto>> GetMyReflectionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
