using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Categorias;
using TorneoDeportivo.Application.Features.Categorias.Commands.UpdateCategoria;

namespace TorneoDeportivo.API.Endpoints.Categorias;

/// <summary>
/// Endpoint que edita una categoría existente de un torneo. Solo accesible para el rol Coordinador.
/// </summary>
public class UpdateCategoriaEndpoint(ISender sender) : Endpoint<UpdateCategoriaRequest, CategoriaResponse>
{
    /// <summary>
    /// Configura la ruta PUT /api/v1/torneos/{torneoId}/categorias/{id}, restringida al rol Coordinador.
    /// </summary>
    public override void Configure()
    {
        Put("/api/v1/torneos/{torneoId}/categorias/{id}");
        Roles("Coordinador");
    }

    /// <summary>
    /// Envía el <see cref="UpdateCategoriaCommand"/> y devuelve la categoría actualizada.
    /// </summary>
    public override async Task HandleAsync(UpdateCategoriaRequest req, CancellationToken ct)
    {
        var command = new UpdateCategoriaCommand(
            req.Id,
            req.TorneoId,
            req.Nombre,
            req.TipoCompetencia,
            req.Sexo,
            req.RangoEdadMin,
            req.RangoEdadMax,
            req.RangoPesoMin,
            req.RangoPesoMax,
            req.RangoGraduacionMin,
            req.RangoGraduacionMax);

        var result = await sender.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}
