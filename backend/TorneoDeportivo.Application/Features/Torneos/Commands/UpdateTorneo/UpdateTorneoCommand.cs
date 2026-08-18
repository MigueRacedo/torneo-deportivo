using MediatR;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.UpdateTorneo;

/// <summary>
/// Comando CQRS que solicita la edición de los datos de un torneo existente (H0003).
/// </summary>
public record UpdateTorneoCommand(Guid Id, string Nombre, DateOnly Fecha, string Lugar, string? ImagenFlyer)
    : IRequest<TorneoResponse>;
