namespace TorneoDeportivo.Application.Common.Interfaces;

/// <summary>
/// Abstracción para notificar en vivo (SignalR) que un match de un torneo cambió, sin acoplar la capa Application
/// a la infraestructura de tiempo real. La implementación vive en la capa API.
/// </summary>
public interface IBracketNotifier
{
    /// <summary>Notifica a los clientes del torneo que un match se actualizó (e.g. se registró un ganador).</summary>
    Task NotificarMatchActualizadoAsync(Guid torneoId, Guid categoriaId, CancellationToken ct);
}
