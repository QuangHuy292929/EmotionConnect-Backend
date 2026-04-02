using Application.Interfaces.Common;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

[Authorize]
public class PresenceHub : Hub
{
    private readonly IUserPresenceService _userPresenceService;

    public PresenceHub(IUserPresenceService userPresenceService)
    {
        _userPresenceService = userPresenceService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.GetCurrentUserId()
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        await _userPresenceService.UserConnectedAsync(userId, Context.ConnectionId);

        await Clients.All.SendAsync("UserOnline", new
        {
            userId,
            isOnline = true
        });

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = Context.User;
        if (user is not null)
        {
            try
            {
                var userId = user.GetCurrentUserId();
                await _userPresenceService.UserDisconnectedAsync(userId, Context.ConnectionId);

                await Clients.All.SendAsync("UserOffline", new
                {
                    userId,
                    isOnline = false
                });
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore invalid auth state during disconnect cleanup.
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}
