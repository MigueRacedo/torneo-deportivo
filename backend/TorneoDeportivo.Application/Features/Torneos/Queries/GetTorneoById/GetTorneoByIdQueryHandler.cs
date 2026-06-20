using Mapster;
using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneoById;

public class GetTorneoByIdQueryHandler(ITorneoRepository repo) : IRequestHandler<GetTorneoByIdQuery, TorneoResponse>
{
    public async Task<TorneoResponse> Handle(GetTorneoByIdQuery request, CancellationToken ct)
    {
        var torneo = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Torneo), request.Id);

        return torneo.Adapt<TorneoResponse>();
    }
}
