using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Competidores.Commands.DeleteCompetidor;

namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Endpoint que elimina un competidor de un torneo. Solo accesible para el rol Coordinador.
/// </summary>
public class DeleteCompetidorEndpoint(ISender sender) : Endpoint<DeleteCompetidorRequest>
{
    /// <summary>
    /// Configura la ruta DELETE /api/v1/torneos/{torneoId}/competidores/{id}, restringida al rol Coordinador.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/v1/torneos/{torneoId}/competidores/{id}");
        Roles("Coordinador");
    }

    /// <summary>
    /// Envía el <see cref="DeleteCompetidorCommand"/> y responde 204 No Content si la eliminación fue exitosa.
    /// </summary>
    public override async Task HandleAsync(DeleteCompetidorRequest req, CancellationToken ct)
    {
        await sender.Send(new DeleteCompetidorCommand(req.Id, req.TorneoId), ct);
        await Send.NoContentAsync(ct);
    }
}
