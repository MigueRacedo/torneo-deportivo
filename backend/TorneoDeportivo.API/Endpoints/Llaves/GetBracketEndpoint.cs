using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Llaves;
using TorneoDeportivo.Application.Features.Llaves.Queries.GetBracket;

namespace TorneoDeportivo.API.Endpoints.Llaves;

/// <summary>
/// Endpoint que devuelve el bracket de una categoría, accesible para todos los roles del sistema.
/// </summary>
public class GetBracketEndpoint(ISender sender) : Endpoint<GetBracketRequest, BracketResponse>
{
    /// <summary>
    /// Configura la ruta GET /api/v1/torneos/{torneoId}/llaves/{categoriaId}, habilitada para Coordinador,
    /// Profesor y Director.
    /// </summary>
    public override void Configure()
    {
        Get("/api/v1/torneos/{torneoId}/llaves/{categoriaId}");
        Roles("Coordinador", "Profesor", "Director");
    }

    /// <summary>
    /// Envía el <see cref="GetBracketQuery"/> y devuelve el bracket de la categoría.
    /// </summary>
    public override async Task HandleAsync(GetBracketRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetBracketQuery(req.TorneoId, req.CategoriaId), ct);
        await Send.OkAsync(result, ct);
    }
}
