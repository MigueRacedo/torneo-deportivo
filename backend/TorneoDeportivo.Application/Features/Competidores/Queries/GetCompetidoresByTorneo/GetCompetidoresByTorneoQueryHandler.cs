using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Competidores.Queries.GetCompetidoresByTorneo;

/// <summary>
/// Handler CQRS que obtiene los competidores de un torneo, validando primero que el torneo exista,
/// y los mapea a su representación de respuesta (con el nombre de la categoría de cada uno).
/// </summary>
public class GetCompetidoresByTorneoQueryHandler(
    ICompetidorRepository competidorRepo,
    ITorneoRepository torneoRepo) : IRequestHandler<GetCompetidoresByTorneoQuery, List<CompetidorResponse>>
{
    /// <summary>
    /// Verifica la existencia del torneo, consulta sus competidores y los devuelve mapeados a
    /// <see cref="CompetidorResponse"/>.
    /// </summary>
    public async Task<List<CompetidorResponse>> Handle(GetCompetidoresByTorneoQuery request, CancellationToken ct)
    {
        var torneo = await torneoRepo.GetByIdAsync(request.TorneoId, ct)
            ?? throw new NotFoundException(nameof(Torneo), request.TorneoId);

        var competidores = await competidorRepo.GetByTorneoIdAsync(torneo.Id, ct);
        return competidores.Select(c => c.ToResponse()).ToList();
    }
}
