using MediatR;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.CambiarEstadoTorneo;

/// <summary>
/// Comando CQRS que mueve un torneo a otro estado del ciclo de vida (H0009).
/// </summary>
/// <param name="NuevoEstado">Estado destino, como texto ("Borrador", "Activo" o "Finalizado").</param>
/// <param name="Forzar">
/// Permite finalizar un torneo que todavía tiene categorías sin resolver. Es una decisión del
/// Coordinador (una categoría puede quedar sin disputarse), no un atajo para saltear validaciones:
/// no habilita ninguna otra transición.
/// </param>
public record CambiarEstadoTorneoCommand(Guid TorneoId, string NuevoEstado, bool Forzar = false)
    : IRequest<TorneoResponse>;
