using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Competidores;
using TorneoDeportivo.Application.Features.Competidores.Commands.CreateCompetidor;

namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Endpoint que carga un competidor en una categoría de un torneo (H0004). Solo rol Coordinador.
/// </summary>
public class CreateCompetidorEndpoint(ISender sender) : Endpoint<CreateCompetidorRequest, CompetidorResponse>
{
    /// <summary>
    /// Configura la ruta POST /api/v1/torneos/{torneoId}/competidores, restringida al rol Coordinador,
    /// y declara la respuesta 201.
    /// </summary>
    public override void Configure()
    {
        Post("/api/v1/torneos/{torneoId}/competidores");
        Roles("Coordinador");
        Description(b => b.Produces<CompetidorResponse>(201));
    }

    /// <summary>
    /// Envía el <see cref="CreateCompetidorCommand"/> y responde con 201 Created con el competidor cargado.
    /// </summary>
    public override async Task HandleAsync(CreateCompetidorRequest req, CancellationToken ct)
    {
        var command = new CreateCompetidorCommand(
            req.TorneoId,
            req.Nombre,
            req.Apellido,
            req.Sexo,
            req.Edad,
            req.Graduacion,
            req.Peso,
            req.Altura,
            req.Escuela,
            req.Responsable,
            req.Telefono);

        var result = await sender.Send(command, ct);
        await Send.CreatedAtAsync<GetCompetidoresByTorneoEndpoint>(
            new { torneoId = result.TorneoId }, result, cancellation: ct);
    }
}
