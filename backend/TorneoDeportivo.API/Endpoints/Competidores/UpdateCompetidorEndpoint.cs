using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Competidores;
using TorneoDeportivo.Application.Features.Competidores.Commands.UpdateCompetidor;

namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Endpoint que edita un competidor de un torneo. Solo accesible para el rol Coordinador.
/// </summary>
public class UpdateCompetidorEndpoint(ISender sender) : Endpoint<UpdateCompetidorRequest, CompetidorResponse>
{
    /// <summary>
    /// Configura la ruta PUT /api/v1/torneos/{torneoId}/competidores/{id}, restringida al rol Coordinador.
    /// </summary>
    public override void Configure()
    {
        Put("/api/v1/torneos/{torneoId}/competidores/{id}");
        Roles("Coordinador");
    }

    /// <summary>
    /// Envía el <see cref="UpdateCompetidorCommand"/> y devuelve el competidor actualizado.
    /// </summary>
    public override async Task HandleAsync(UpdateCompetidorRequest req, CancellationToken ct)
    {
        var command = new UpdateCompetidorCommand(
            req.Id,
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
        await Send.OkAsync(result, ct);
    }
}
