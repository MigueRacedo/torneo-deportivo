using MediatR;

namespace TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneos;

/// <summary>
/// Query CQRS que solicita el listado de torneos activos.
/// </summary>
public record GetTorneosQuery : IRequest<List<TorneoResponse>>;
