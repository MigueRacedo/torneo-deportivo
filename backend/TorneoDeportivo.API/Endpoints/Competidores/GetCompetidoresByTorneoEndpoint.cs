using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Competidores;
using TorneoDeportivo.Application.Features.Competidores.Queries.GetCompetidoresByTorneo;

namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Endpoint que lista los competidores inscriptos en un torneo. Solo accesible para el rol Coordinador.
/// </summary>
public class GetCompetidoresByTorneoEndpoint(ISender sender)
    : Endpoint<GetCompetidoresByTorneoRequest, List<CompetidorResponse>>
{
    /// <summary>
    /// Configura la ruta GET /api/v1/torneos/{torneoId}/competidores, restringida al rol Coordinador.
    /// </summary>
    public override void Configure()
    {
        Get("/api/v1/torneos/{torneoId}/competidores");
        Roles("Coordinador");
    }

    /// <summary>
    /// Envía el <see cref="GetCompetidoresByTorneoQuery"/> y devuelve el listado de competidores del torneo.
    /// </summary>
    public override async Task HandleAsync(GetCompetidoresByTorneoRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetCompetidoresByTorneoQuery(req.TorneoId), ct);
        await Send.OkAsync(result, ct);
    }
}
