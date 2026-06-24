using backend.DTOs;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace backend.Services;

public class NotificacionService(IHubContext<NotificacionHub> hub)
{
    public Task NotificarGTIAdminAsync(string tipo, string titulo, string mensaje) =>
        hub.Clients.Group("GTIAdmin").SendAsync("Notificacion",
            new NotificacionDto(tipo, titulo, mensaje, DateTime.Now));

    public Task NotificarAdminAsync(string tipo, string titulo, string mensaje) =>
        hub.Clients.Group("Admin").SendAsync("Notificacion",
            new NotificacionDto(tipo, titulo, mensaje, DateTime.Now));
}
