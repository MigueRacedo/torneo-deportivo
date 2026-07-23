using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Torneos;
using TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneoById;

namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Endpoint que devuelve el detalle de un torneo por su id, accesible para todos los roles del sistema.
/// </summary>
public class GetTorneoByIdEndpoint(ISender sender) : Endpoint<GetTorneoByIdRequest, TorneoResponse>
{
    /// <summary>
    /// Configura la ruta GET /api/v1/torneos/{id}, habilitada para Coordinador, Profesor y Director.
    /// </summary>
    public override void Configure()
    {
        Get("/api/v1/torneos/{id}");
        Roles("Coordinador", "Profesor", "Director");
    }

    /// <summary>
    /// Envía el <see cref="GetTorneoByIdQuery"/> y devuelve el torneo encontrado.
    /// </summary>
    public override async Task HandleAsync(GetTorneoByIdRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetTorneoByIdQuery(req.Id), ct);
        await Send.OkAsync(result, ct);
    }
}
