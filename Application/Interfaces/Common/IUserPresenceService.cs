namespace Application.Interfaces.Common;

public interface IUserPresenceService
{
    Task UserConnectedAsync(Guid userId, string connectionId, CancellationToken cancellationToken = default);
    Task UserDisconnectedAsync(Guid userId, string connectionId, CancellationToken cancellationToken = default);
    Task UpdateLastActiveAsync(Guid userId, CancellationToken cancellationToken = default);
    // lấy trạng thái online của user, dựa trên việc có connection nào đang tồn tại hay không.
    Task<bool> IsOnlineAsync(Guid userId, CancellationToken cancellationToken = default);
    // lấy tất cả userId đang online bằng cách scan Redis keys theo pattern. Cần thiết vì không có cách nào khác để biết được tất cả userId nào đang online, do trạng thái online được lưu riêng biệt cho từng userId.
    Task<IEnumerable<string>> GetOnlineUserIdsAsync(CancellationToken cancellationToken = default);

    Task<DateTime?> GetLastActiveAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetActiveConnectionCountAsync(Guid userId, CancellationToken cancellationToken = default);
}
