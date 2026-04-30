using Application.DTOs.OutboxMessage;

namespace Application.Interfaces.IServices;

public interface IOutboxMessageService
{
    Task<OutboxMessageDto> EnqueueAsync(CreateOutboxMessageRequestDto request, CancellationToken cancellationToken = default);
    Task<List<OutboxMessageDto>> EnqueueRangeAsync(IEnumerable<CreateOutboxMessageRequestDto> requests, CancellationToken cancellationToken = default);
    Task<OutboxMessageDto> GetByIdAsync(Guid outboxMessageId, CancellationToken cancellationToken = default);
    Task<List<OutboxMessageDto>> GetPendingAsync(int take = 50, CancellationToken cancellationToken = default);
    Task<OutboxMessageDto> MarkProcessingAsync(Guid outboxMessageId, CancellationToken cancellationToken = default);
    Task<OutboxMessageDto> MarkProcessedAsync(Guid outboxMessageId, CancellationToken cancellationToken = default);
    Task<OutboxMessageDto> MarkFailedAsync(Guid outboxMessageId, string? errorMessage, CancellationToken cancellationToken = default);
}
