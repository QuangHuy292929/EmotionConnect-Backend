using Application.DTOs.CheckIn;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IServices;

public interface ICheckInSessionService
{
    Task<CheckInStartResponseDto> StartAsync(Guid userId, StartCheckInRequest request, CancellationToken ct = default);
    Task<CheckInSessionDto?> GetActiveSessionAsync(Guid userId, CancellationToken ct = default);
    Task<CheckInSessionDto?> GetBySessionId(Guid sessionId, CancellationToken ct = default);
    Task<CheckInStepResponseDto> SubmitAnswerAsync(Guid sessionId, Guid userId, SubmitCheckInAnswerRequest request, CancellationToken ct = default);
    Task<List<CheckInSessionDto>> GetMySessions(Guid userId, CancellationToken ct = default);
    Task<CheckInCompletedDto> ConfirmAsync(Guid sessionId, Guid userId, ConfirmCheckInRequest request, CancellationToken ct = default);
    Task CancelAsync(Guid sessionId, Guid userId, CancellationToken ct = default);
}
