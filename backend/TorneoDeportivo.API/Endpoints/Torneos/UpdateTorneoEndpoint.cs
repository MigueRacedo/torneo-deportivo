using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Torneos;
using TorneoDeportivo.Application.Features.Torneos.Commands.UpdateTorneo;

namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Endpoint que edita los datos de un torneo existente (H0003). Solo accesible para el rol Coordinador.
/// </summary>
public class UpdateTorneoEndpoint(ISender sender) : Endpoint<UpdateTorneoRequest, TorneoResponse>
{
    /// <summary>
    /// Configura la ruta PUT /api/v1/torneos/{id}, restringida al rol Coordinador.
    /// </summary>
    public override void Configure()
    {
        Put("/api/v1/torneos/{id}");
        Roles("Coordinador");
    }

    /// <summary>
    /// Envía el <see cref="UpdateTorneoCommand"/> y devuelve el torneo actualizado.
    /// </summary>
    public override async Task HandleAsync(UpdateTorneoRequest req, CancellationToken ct)
    {
        var command = new UpdateTorneoCommand(req.Id, req.Nombre, req.Fecha, req.Lugar, req.ImagenFlyer);
        var result = await sender.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}
