using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Torneos;
using TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneos;

namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Endpoint que lista los torneos activos, accesible para todos los roles del sistema.
/// </summary>
public class GetTorneosEndpoint(ISender sender) : EndpointWithoutRequest<List<TorneoResponse>>
{
    /// <summary>
    /// Configura la ruta GET /api/v1/torneos, habilitada para Coordinador, Profesor y Director.
    /// </summary>
    public override void Configure()
    {
        Get("/api/v1/torneos");
        Roles("Coordinador", "Profesor", "Director");
    }

    /// <summary>
    /// Envía el <see cref="GetTorneosQuery"/> y devuelve el listado de torneos.
    /// </summary>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetTorneosQuery(), ct);
        await Send.OkAsync(result, ct);
    }
}
