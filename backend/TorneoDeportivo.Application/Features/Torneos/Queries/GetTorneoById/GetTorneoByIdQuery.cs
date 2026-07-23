using MediatR;

namespace TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneoById;

/// <summary>
/// Query CQRS que solicita el detalle de un torneo por su id.
/// </summary>
public record GetTorneoByIdQuery(Guid Id) : IRequest<TorneoResponse>;
