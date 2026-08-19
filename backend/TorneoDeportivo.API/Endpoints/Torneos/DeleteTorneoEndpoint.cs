using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Torneos.Commands.DeleteTorneo;

namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Endpoint que elimina un torneo en estado Borrador. Solo accesible para el rol Coordinador.
/// </summary>
public class DeleteTorneoEndpoint(ISender sender) : Endpoint<DeleteTorneoRequest>
{
    /// <summary>
    /// Configura la ruta DELETE /api/v1/torneos/{id}, restringida al rol Coordinador.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/v1/torneos/{id}");
        Roles("Coordinador");
    }

    /// <summary>
    /// Envía el <see cref="DeleteTorneoCommand"/> y responde 204 No Content si la eliminación fue exitosa.
    /// </summary>
    public override async Task HandleAsync(DeleteTorneoRequest req, CancellationToken ct)
    {
        await sender.Send(new DeleteTorneoCommand(req.Id), ct);
        await Send.NoContentAsync(ct);
    }
}
