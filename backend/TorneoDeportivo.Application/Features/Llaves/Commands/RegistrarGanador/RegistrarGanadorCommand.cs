using MediatR;

namespace TorneoDeportivo.Application.Features.Llaves.Commands.RegistrarGanador;

/// <summary>
/// Comando CQRS que registra el ganador de un match del bracket y lo avanza a la ronda siguiente.
/// </summary>
public record RegistrarGanadorCommand(Guid TorneoId, Guid LlaveId, Guid GanadorId) : IRequest<MatchResponse>;
