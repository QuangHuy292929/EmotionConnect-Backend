using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebAPI.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User != null ? Context.User.GetCurrentUserId() : Guid.Empty;

        await Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId));
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = Context.User;
        if (user is not null)
        {
            var userId = user.GetCurrentUserId();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetUserGroupName(userId));
        }

        await base.OnDisconnectedAsync(exception);
    }

    public static string GetUserGroupName(Guid userId) => $"user:{userId}:notifications";
}
