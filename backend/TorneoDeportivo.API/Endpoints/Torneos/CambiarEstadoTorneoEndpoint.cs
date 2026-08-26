using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Torneos;
using TorneoDeportivo.Application.Features.Torneos.Commands.CambiarEstadoTorneo;

namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Endpoint que mueve un torneo por su ciclo de vida: Borrador → Activo → Finalizado (H0009).
/// Solo rol Coordinador.
/// </summary>
public class CambiarEstadoTorneoEndpoint(ISender sender) : Endpoint<CambiarEstadoTorneoRequest, TorneoResponse>
{
    /// <summary>
    /// Configura la ruta PUT /api/v1/torneos/{id}/estado, restringida al rol Coordinador.
    /// </summary>
    public override void Configure()
    {
        Put("/api/v1/torneos/{id}/estado");
        Roles("Coordinador");
    }

    /// <summary>
    /// Envía el <see cref="CambiarEstadoTorneoCommand"/> y devuelve el torneo con su estado actualizado.
    /// </summary>
    public override async Task HandleAsync(CambiarEstadoTorneoRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new CambiarEstadoTorneoCommand(req.Id, req.Estado, req.Forzar), ct);
        await Send.OkAsync(result, ct);
    }
}
