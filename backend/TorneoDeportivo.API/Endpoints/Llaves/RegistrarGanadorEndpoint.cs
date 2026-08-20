using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Llaves;
using TorneoDeportivo.Application.Features.Llaves.Commands.RegistrarGanador;

namespace TorneoDeportivo.API.Endpoints.Llaves;

/// <summary>
/// Endpoint que registra el ganador de un match del bracket (H0005). Solo rol Coordinador.
/// </summary>
public class RegistrarGanadorEndpoint(ISender sender) : Endpoint<RegistrarGanadorRequest, MatchResponse>
{
    /// <summary>
    /// Configura la ruta PUT /api/v1/torneos/{torneoId}/llaves/{id}/ganador, restringida al rol Coordinador.
    /// </summary>
    public override void Configure()
    {
        Put("/api/v1/torneos/{torneoId}/llaves/{id}/ganador");
        Roles("Coordinador");
    }

    /// <summary>
    /// Envía el <see cref="RegistrarGanadorCommand"/> y devuelve el match actualizado.
    /// </summary>
    public override async Task HandleAsync(RegistrarGanadorRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new RegistrarGanadorCommand(req.TorneoId, req.Id, req.GanadorId), ct);
        await Send.OkAsync(result, ct);
    }
}
