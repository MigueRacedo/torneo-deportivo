using MediatR;

namespace TorneoDeportivo.Application.Features.Llaves.Queries.GetBracket;

/// <summary>
/// Query CQRS que solicita el bracket (llaves) de una categoría de un torneo.
/// </summary>
public record GetBracketQuery(Guid TorneoId, Guid CategoriaId) : IRequest<BracketResponse>;
