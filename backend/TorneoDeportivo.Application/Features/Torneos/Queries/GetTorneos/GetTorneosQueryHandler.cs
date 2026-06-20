using Mapster;
using MediatR;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneos;

public class GetTorneosQueryHandler(ITorneoRepository repo) : IRequestHandler<GetTorneosQuery, List<TorneoResponse>>
{
    public async Task<List<TorneoResponse>> Handle(GetTorneosQuery request, CancellationToken ct)
    {
        var torneos = await repo.GetAllActivosAsync(ct);
        return torneos.Adapt<List<TorneoResponse>>();
    }
}
