using MediatR;

namespace TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneoById;

public record GetTorneoByIdQuery(Guid Id) : IRequest<TorneoResponse>;
