using Application.DTOs.OutboxMessage;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.IServices;
using Domain.Entities;
using Domain.Enums;
using Infracstructure.Mappers;

namespace Infracstructure.Services;

public class OutboxMessageService : IOutboxMessageService
{
    private readonly IUnitOfWork _unitOfWork;

    public OutboxMessageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OutboxMessageDto> EnqueueAsync(
        CreateOutboxMessageRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidatePayload(request.PayloadJson);

        var message = new OutBoxMessage
        {
            EventType = request.EventType,
            AggregateType = request.AggregateType,
            AggregateId = request.AggregateId,
            PayloadJson = request.PayloadJson.Trim(),
            OccurredAt = request.OccurredAt ?? DateTime.UtcNow,
            Status = OutBoxStatus.Pending,
            RetryCount = 0
        };

        await _unitOfWork.OutboxMessageRepository.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);

        return message.ToDto();
    }

    public async Task<List<OutboxMessageDto>> EnqueueRangeAsync(
        IEnumerable<CreateOutboxMessageRequestDto> requests,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);

        var requestList = requests.ToList();
        foreach (var request in requestList)
        {
            if (request is null)
            {
                throw new BadRequestException("Outbox request cannot be null.");
            }

            ValidatePayload(request.PayloadJson);
        }

        var messages = requestList.Select(request => new OutBoxMessage
        {
            EventType = request.EventType,
            AggregateType = request.AggregateType,
            AggregateId = request.AggregateId,
            PayloadJson = request.PayloadJson.Trim(),
            OccurredAt = request.OccurredAt ?? DateTime.UtcNow,
            Status = OutBoxStatus.Pending,
            RetryCount = 0
        }).ToList();

        await _unitOfWork.OutboxMessageRepository.AddRangeAsync(messages, cancellationToken);
        await _unitOfWork.SaveChangeAsync(cancellationToken);

        return messages.ToDtoList();
    }

    public async Task<OutboxMessageDto> GetByIdAsync(Guid outboxMessageId, CancellationToken cancellationToken = default)
    {
        ValidateId(outboxMessageId);

        var message = await _unitOfWork.OutboxMessageRepository.GetByIdAsync(outboxMessageId, cancellationToken);
        if (message is null)
        {
            throw new NotFoundException("Outbox message not found.");
        }

        return message.ToDto();
    }

    public async Task<List<OutboxMessageDto>> GetPendingAsync(int take = 50, CancellationToken cancellationToken = default)
    {
        if (take <= 0)
        {
            throw new BadRequestException("Take must be greater than zero.");
        }

        var messages = await _unitOfWork.OutboxMessageRepository.GetByStatusAsync(
            OutBoxStatus.Pending,
            take,
            cancellationToken);

        return messages.ToDtoList();
    }

    public async Task<OutboxMessageDto> MarkProcessingAsync(Guid outboxMessageId, CancellationToken cancellationToken = default)
    {
        var message = await GetTrackedMessageAsync(outboxMessageId, cancellationToken);

        if (message.Status == OutBoxStatus.Processed)
        {
            throw new ConflictException("Processed outbox messages cannot be moved back to processing.");
        }

        message.Status = OutBoxStatus.Processing;
        message.LastError = null;
        message.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return message.ToDto();
    }

    public async Task<OutboxMessageDto> MarkProcessedAsync(Guid outboxMessageId, CancellationToken cancellationToken = default)
    {
        var message = await GetTrackedMessageAsync(outboxMessageId, cancellationToken);

        message.Status = OutBoxStatus.Processed;
        message.ProcessedAt = DateTime.UtcNow;
        message.LastError = null;
        message.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return message.ToDto();
    }

    public async Task<OutboxMessageDto> MarkFailedAsync(
        Guid outboxMessageId,
        string? errorMessage,
        CancellationToken cancellationToken = default)
    {
        var message = await GetTrackedMessageAsync(outboxMessageId, cancellationToken);

        message.Status = OutBoxStatus.Failed;
        message.RetryCount += 1;
        message.LastError = NormalizeOptional(errorMessage);
        message.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangeAsync(cancellationToken);
        return message.ToDto();
    }

    private async Task<OutBoxMessage> GetTrackedMessageAsync(Guid outboxMessageId, CancellationToken cancellationToken)
    {
        ValidateId(outboxMessageId);

        var message = await _unitOfWork.OutboxMessageRepository.GetByIdAsync(outboxMessageId, cancellationToken);
        if (message is null)
        {
            throw new NotFoundException("Outbox message not found.");
        }

        return message;
    }

    private static void ValidateId(Guid outboxMessageId)
    {
        if (outboxMessageId == Guid.Empty)
        {
            throw new BadRequestException("Outbox message ID cannot be empty.");
        }
    }

    private static void ValidatePayload(string payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            throw new BadRequestException("PayloadJson cannot be null or empty.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
