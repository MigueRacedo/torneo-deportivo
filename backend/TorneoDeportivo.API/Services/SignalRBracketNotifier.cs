using Microsoft.AspNetCore.SignalR;
using TorneoDeportivo.API.Hubs;
using TorneoDeportivo.Application.Common.Interfaces;

namespace TorneoDeportivo.API.Services;

/// <summary>
/// Implementación de <see cref="IBracketNotifier"/> sobre SignalR: emite el evento "MatchActualizado" al grupo
/// del torneo para que los clientes conectados refresquen el bracket en vivo.
/// </summary>
public class SignalRBracketNotifier(IHubContext<BracketHub> hub) : IBracketNotifier
{
    /// <summary>Envía el evento "MatchActualizado" al grupo "torneo-{torneoId}".</summary>
    public Task NotificarMatchActualizadoAsync(Guid torneoId, Guid categoriaId, CancellationToken ct) =>
        hub.Clients.Group($"torneo-{torneoId}")
            .SendAsync("MatchActualizado", new { torneoId, categoriaId }, ct);
}
