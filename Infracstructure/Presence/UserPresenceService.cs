using Application.Interfaces.Common;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infracstructure.Presence;

public class UserPresenceService : IUserPresenceService
{
    private readonly IDatabase _database;
    private readonly RedisPresenceOptions _options;

    public UserPresenceService(IConnectionMultiplexer connectionMultiplexer, IOptions<RedisPresenceOptions> options)
    {
        _database = connectionMultiplexer.GetDatabase();
        _options = options.Value;
    }

    public async Task UserConnectedAsync(Guid userId, string connectionId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        ValidateConnectionId(connectionId);

        var connectionsKey = GetConnectionsKey(userId);
        var lastActiveKey = GetLastActiveKey(userId);
        var now = DateTime.UtcNow;

        await _database.SetAddAsync(connectionsKey, connectionId);
        await _database.KeyExpireAsync(connectionsKey, TimeSpan.FromHours(_options.ConnectionTtlHours));
        await _database.StringSetAsync(lastActiveKey, now.ToString("O"), TimeSpan.FromMinutes(_options.LastActiveTtlMinutes));
    }

    public async Task UserDisconnectedAsync(Guid userId, string connectionId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);
        ValidateConnectionId(connectionId);

        var connectionsKey = GetConnectionsKey(userId);
        var lastActiveKey = GetLastActiveKey(userId);
        var now = DateTime.UtcNow;

        await _database.SetRemoveAsync(connectionsKey, connectionId);

        var remainingConnections = await _database.SetLengthAsync(connectionsKey);
        if (remainingConnections <= 0)
        {
            await _database.KeyDeleteAsync(connectionsKey);
        }
        else
        {
            await _database.KeyExpireAsync(connectionsKey, TimeSpan.FromHours(_options.ConnectionTtlHours));
        }

        await _database.StringSetAsync(lastActiveKey, now.ToString("O"), TimeSpan.FromMinutes(_options.LastActiveTtlMinutes));
    }

    public async Task UpdateLastActiveAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);

        var lastActiveKey = GetLastActiveKey(userId);
        var now = DateTime.UtcNow;

        await _database.StringSetAsync(lastActiveKey, now.ToString("O"), TimeSpan.FromMinutes(_options.LastActiveTtlMinutes));
    }

    public async Task<bool> IsOnlineAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);

        var connectionCount = await GetActiveConnectionCountAsync(userId, cancellationToken);
        return connectionCount > 0;
    }

    public async Task<DateTime?> GetLastActiveAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);

        var lastActiveValue = await _database.StringGetAsync(GetLastActiveKey(userId));
        if (!lastActiveValue.HasValue)
        {
            return null;
        }

        return DateTime.TryParse(lastActiveValue!, out var parsed)
            ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
            : null;
    }

    public async Task<int> GetActiveConnectionCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        ValidateUserId(userId);

        var count = await _database.SetLengthAsync(GetConnectionsKey(userId));
        return (int)count;
    }

    private string GetConnectionsKey(Guid userId)
    {
        return $"{_options.KeyPrefix}:user:{userId}:connections";
    }

    private string GetLastActiveKey(Guid userId)
    {
        return $"{_options.KeyPrefix}:user:{userId}:last-active";
    }

    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.", nameof(userId));
        }
    }

    private static void ValidateConnectionId(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            throw new ArgumentException("ConnectionId is required.", nameof(connectionId));
        }
    }
}
