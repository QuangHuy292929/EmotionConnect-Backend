using Application.Interfaces.Common;
using Infracstructure.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Middlewares;

public class UserActivityMiddleware
{
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public UserActivityMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context, IUserPresenceService userPresenceService)
    {
        if (ShouldTrack(context))
        {
            var userId = context.User.GetCurrentUserId();
            var cacheKey = $"presence:last-active:{userId}";

            if (!_cache.TryGetValue<DateTime>(cacheKey, out var lastUpdated) ||
                DateTime.UtcNow - lastUpdated > UpdateInterval)
            {
                await userPresenceService.UpdateLastActiveAsync(userId, context.RequestAborted);
                _cache.Set(cacheKey, DateTime.UtcNow, CacheTtl);
            }
        }

        await _next(context);
    }

    private static bool ShouldTrack(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        if (context.WebSockets.IsWebSocketRequest)
        {
            return false;
        }

        if (context.Request.Path.StartsWithSegments("/hubs"))
        {
            return false;
        }

        return true;
    }
}
