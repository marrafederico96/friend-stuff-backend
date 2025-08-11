using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FriendStuffBackend.Features.EventMessage;

 [Authorize]
public class MessageHub : Hub
{
    public async Task JoinEventGroup(string eventName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, eventName);
    }

    public async Task LeaveEventGroup(string eventName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventName);
    }
}