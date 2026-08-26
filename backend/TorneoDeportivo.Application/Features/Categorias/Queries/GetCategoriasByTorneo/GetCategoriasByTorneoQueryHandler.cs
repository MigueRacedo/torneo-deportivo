using Mapster;
using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Categorias.Queries.GetCategoriasByTorneo;

/// <summary>
/// Handler CQRS que obtiene las categorías de un torneo junto con cuántos competidores tiene cada una,
/// validando primero que el torneo exista.
/// </summary>
public class GetCategoriasByTorneoQueryHandler(
    ICategoriaRepository categoriaRepo,
    ICompetidorRepository competidorRepo,
    ITorneoRepository torneoRepo) : IRequestHandler<GetCategoriasByTorneoQuery, List<CategoriaResponse>>
{
    /// <summary>
    /// Verifica la existencia del torneo, consulta sus categorías y les agrega el total de competidores
    /// asignados a cada una (dato central de la consulta del Profesor, H0006).
    /// </summary>
    public async Task<List<CategoriaResponse>> Handle(GetCategoriasByTorneoQuery request, CancellationToken ct)
    {
        var torneo = await torneoRepo.GetByIdAsync(request.TorneoId, ct)
            ?? throw new NotFoundException(nameof(Torneo), request.TorneoId);

        var categorias = await categoriaRepo.GetByTorneoIdAsync(torneo.Id, ct);

        // Un único GROUP BY para todas las categorías, en lugar de una consulta de conteo por cada una.
        var conteo = await competidorRepo.GetConteoPorCategoriaAsync(torneo.Id, ct);

        return categorias
            .Select(c => c.Adapt<CategoriaResponse>() with
            {
                TotalCompetidores = conteo.GetValueOrDefault(c.Id)
            })
            .ToList();
    }
}
