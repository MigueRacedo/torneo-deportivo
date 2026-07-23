using Mapster;
using MediatR;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneos;

/// <summary>
/// Handler CQRS que obtiene todos los torneos activos y los mapea a su representación de respuesta.
/// </summary>
public class GetTorneosQueryHandler(ITorneoRepository repo) : IRequestHandler<GetTorneosQuery, List<TorneoResponse>>
{
    /// <summary>
    /// Consulta los torneos activos en el repositorio y los devuelve mapeados a <see cref="TorneoResponse"/>.
    /// </summary>
    public async Task<List<TorneoResponse>> Handle(GetTorneosQuery request, CancellationToken ct)
    {
        var torneos = await repo.GetAllActivosAsync(ct);
        return torneos.Adapt<List<TorneoResponse>>();
    }
}
