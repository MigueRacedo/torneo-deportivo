using MediatR;

namespace TorneoDeportivo.Application.Features.Competidores.Queries.GetCompetidoresByTorneo;

/// <summary>
/// Query CQRS que solicita el listado de competidores inscriptos en un torneo (vista del Coordinador).
/// </summary>
public record GetCompetidoresByTorneoQuery(Guid TorneoId) : IRequest<List<CompetidorResponse>>;
