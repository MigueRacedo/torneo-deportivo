using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Categorias;
using TorneoDeportivo.Application.Features.Categorias.Queries.GetCategoriasByTorneo;

namespace TorneoDeportivo.API.Endpoints.Categorias;

/// <summary>
/// Endpoint que lista las categorías de un torneo, accesible para todos los roles del sistema.
/// </summary>
public class GetCategoriasByTorneoEndpoint(ISender sender)
    : Endpoint<GetCategoriasByTorneoRequest, List<CategoriaResponse>>
{
    /// <summary>
    /// Configura la ruta GET /api/v1/torneos/{torneoId}/categorias, habilitada para Coordinador,
    /// Profesor y Director.
    /// </summary>
    public override void Configure()
    {
        Get("/api/v1/torneos/{torneoId}/categorias");
        Roles("Coordinador", "Profesor", "Director");
    }

    /// <summary>
    /// Envía el <see cref="GetCategoriasByTorneoQuery"/> y devuelve el listado de categorías del torneo.
    /// </summary>
    public override async Task HandleAsync(GetCategoriasByTorneoRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetCategoriasByTorneoQuery(req.TorneoId), ct);
        await Send.OkAsync(result, ct);
    }
}
