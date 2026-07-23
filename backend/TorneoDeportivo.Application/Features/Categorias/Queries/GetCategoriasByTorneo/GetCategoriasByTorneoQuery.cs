using MediatR;

namespace TorneoDeportivo.Application.Features.Categorias.Queries.GetCategoriasByTorneo;

/// <summary>
/// Query CQRS que solicita el listado de categorías definidas para un torneo.
/// </summary>
public record GetCategoriasByTorneoQuery(Guid TorneoId) : IRequest<List<CategoriaResponse>>;
