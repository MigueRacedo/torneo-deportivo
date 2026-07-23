using Microsoft.AspNetCore.SignalR;

namespace TorneoDeportivo.API.Hubs;

/// <summary>
/// Hub de SignalR usado para notificar en vivo actualizaciones del bracket de un torneo a los clientes conectados.
/// </summary>
public class BracketHub : Hub
{
    /// <summary>
    /// Agrega la conexión actual al grupo del torneo indicado para recibir sus actualizaciones en tiempo real.
    /// </summary>
    public async Task JoinTorneo(string torneoId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"torneo-{torneoId}");
}
