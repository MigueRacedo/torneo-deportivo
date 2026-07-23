using Mapster;
using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneoById;

/// <summary>
/// Handler CQRS que busca un torneo por id y lanza <see cref="NotFoundException"/> si no existe.
/// </summary>
public class GetTorneoByIdQueryHandler(ITorneoRepository repo) : IRequestHandler<GetTorneoByIdQuery, TorneoResponse>
{
    /// <summary>
    /// Obtiene el torneo solicitado del repositorio y lo mapea a su representación de respuesta.
    /// </summary>
    public async Task<TorneoResponse> Handle(GetTorneoByIdQuery request, CancellationToken ct)
    {
        var torneo = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Torneo), request.Id);

        return torneo.Adapt<TorneoResponse>();
    }
}
