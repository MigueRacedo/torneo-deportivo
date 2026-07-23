using Mapster;
using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Categorias.Queries.GetCategoriasByTorneo;

/// <summary>
/// Handler CQRS que obtiene las categorías de un torneo, validando primero que el torneo exista,
/// y las mapea a su representación de respuesta.
/// </summary>
public class GetCategoriasByTorneoQueryHandler(
    ICategoriaRepository categoriaRepo,
    ITorneoRepository torneoRepo) : IRequestHandler<GetCategoriasByTorneoQuery, List<CategoriaResponse>>
{
    /// <summary>
    /// Verifica la existencia del torneo, consulta sus categorías en el repositorio y las devuelve
    /// mapeadas a <see cref="CategoriaResponse"/>.
    /// </summary>
    public async Task<List<CategoriaResponse>> Handle(GetCategoriasByTorneoQuery request, CancellationToken ct)
    {
        var torneo = await torneoRepo.GetByIdAsync(request.TorneoId, ct)
            ?? throw new NotFoundException(nameof(Torneo), request.TorneoId);

        var categorias = await categoriaRepo.GetByTorneoIdAsync(torneo.Id, ct);
        return categorias.Adapt<List<CategoriaResponse>>();
    }
}
