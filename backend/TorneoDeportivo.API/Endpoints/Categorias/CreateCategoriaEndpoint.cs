using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Categorias;
using TorneoDeportivo.Application.Features.Categorias.Commands.CreateCategoria;

namespace TorneoDeportivo.API.Endpoints.Categorias;

/// <summary>
/// Endpoint que crea una categoría de competencia dentro de un torneo (H0002). Solo rol Coordinador.
/// </summary>
public class CreateCategoriaEndpoint(ISender sender) : Endpoint<CreateCategoriaRequest, CategoriaResponse>
{
    /// <summary>
    /// Configura la ruta POST /api/v1/torneos/{torneoId}/categorias, restringida al rol Coordinador,
    /// y declara la respuesta 201.
    /// </summary>
    public override void Configure()
    {
        Post("/api/v1/torneos/{torneoId}/categorias");
        Roles("Coordinador");
        Description(b => b.Produces<CategoriaResponse>(201));
    }

    /// <summary>
    /// Envía el <see cref="CreateCategoriaCommand"/> y responde con 201 Created con la categoría creada.
    /// </summary>
    public override async Task HandleAsync(CreateCategoriaRequest req, CancellationToken ct)
    {
        var command = new CreateCategoriaCommand(
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
        await Send.CreatedAtAsync<GetCategoriasByTorneoEndpoint>(
            new { torneoId = result.TorneoId }, result, cancellation: ct);
    }
}
