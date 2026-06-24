using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace backend.Hubs;

[Authorize]
public class NotificacionHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var rol = Context.User?.FindFirstValue(ClaimTypes.Role);
        if (rol is "GTI" or "Administradora")
            await Groups.AddToGroupAsync(Context.ConnectionId, "GTIAdmin");
        if (rol is "Administradora")
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
        await base.OnConnectedAsync();
    }
}
