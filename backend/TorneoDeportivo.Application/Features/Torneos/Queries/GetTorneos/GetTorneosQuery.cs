using MediatR;

namespace TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneos;

public record GetTorneosQuery : IRequest<List<TorneoResponse>>;
