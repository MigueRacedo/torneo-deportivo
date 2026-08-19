using MediatR;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.DeleteTorneo;

/// <summary>
/// Comando CQRS que solicita la eliminación (física) de un torneo. Solo se permite si está en estado Borrador.
/// </summary>
public record DeleteTorneoCommand(Guid Id) : IRequest;
