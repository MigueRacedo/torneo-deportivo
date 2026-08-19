using MediatR;

namespace TorneoDeportivo.Application.Features.Competidores.Commands.DeleteCompetidor;

/// <summary>
/// Comando CQRS que solicita la eliminación (física) de un competidor de un torneo.
/// </summary>
public record DeleteCompetidorCommand(Guid Id, Guid TorneoId) : IRequest;
