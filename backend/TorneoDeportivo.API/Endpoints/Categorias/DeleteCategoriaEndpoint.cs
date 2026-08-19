using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Categorias.Commands.DeleteCategoria;

namespace TorneoDeportivo.API.Endpoints.Categorias;

/// <summary>
/// Endpoint que elimina una categoría de un torneo. Solo accesible para el rol Coordinador.
/// </summary>
public class DeleteCategoriaEndpoint(ISender sender) : Endpoint<DeleteCategoriaRequest>
{
    /// <summary>
    /// Configura la ruta DELETE /api/v1/torneos/{torneoId}/categorias/{id}, restringida al rol Coordinador.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/v1/torneos/{torneoId}/categorias/{id}");
        Roles("Coordinador");
    }

    /// <summary>
    /// Envía el <see cref="DeleteCategoriaCommand"/> y responde 204 No Content si la eliminación fue exitosa.
    /// </summary>
    public override async Task HandleAsync(DeleteCategoriaRequest req, CancellationToken ct)
    {
        await sender.Send(new DeleteCategoriaCommand(req.Id, req.TorneoId), ct);
        await Send.NoContentAsync(ct);
    }
}
