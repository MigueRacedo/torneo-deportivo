using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Llaves.Commands.GenerarLlaves;

namespace TorneoDeportivo.API.Endpoints.Llaves;

/// <summary>
/// Endpoint que clasifica competidores y genera las llaves de un torneo (H0005). Solo rol Coordinador.
/// </summary>
/// <remarks>
/// Es un <c>EndpointWithoutRequest</c> a propósito: el único dato de entrada es el <c>torneoId</c> de la
/// ruta y el POST no lleva cuerpo. Con un DTO de request, FastEndpoints intentaría deserializar el body
/// en un POST y respondería 415 (Unsupported Media Type) porque el cliente no manda
/// <c>Content-Type: application/json</c> cuando no hay cuerpo (ver regla 9 de frontend/CLAUDE.md).
/// </remarks>
public class GenerarLlavesEndpoint(ISender sender) : EndpointWithoutRequest<GenerarLlavesResponse>
{
    /// <summary>
    /// Configura la ruta POST /api/v1/torneos/{torneoId}/llaves/generar, restringida al rol Coordinador.
    /// </summary>
    public override void Configure()
    {
        Post("/api/v1/torneos/{torneoId}/llaves/generar");
        Roles("Coordinador");
    }

    /// <summary>
    /// Envía el <see cref="GenerarLlavesCommand"/> con el id de torneo de la ruta y devuelve el resumen
    /// de la generación por categoría.
    /// </summary>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var torneoId = Route<Guid>("torneoId");
        var result = await sender.Send(new GenerarLlavesCommand(torneoId), ct);
        await Send.OkAsync(result, ct);
    }
}
