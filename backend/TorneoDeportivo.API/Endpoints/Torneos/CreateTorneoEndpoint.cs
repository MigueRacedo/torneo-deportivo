using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Torneos;
using TorneoDeportivo.Application.Features.Torneos.Commands.CreateTorneo;

namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Endpoint que crea un nuevo torneo (H0001). Solo accesible para el rol Coordinador.
/// </summary>
public class CreateTorneoEndpoint(ISender sender) : Endpoint<CreateTorneoRequest, TorneoResponse>
{
    /// <summary>
    /// Configura la ruta POST /api/v1/torneos, restringida al rol Coordinador, y declara la respuesta 201.
    /// </summary>
    public override void Configure()
    {
        Post("/api/v1/torneos");
        Roles("Coordinador");
        Description(b => b.Produces<TorneoResponse>(201));
    }

    /// <summary>
    /// Envía el <see cref="CreateTorneoCommand"/> y responde con 201 Created apuntando al nuevo torneo creado.
    /// </summary>
    public override async Task HandleAsync(CreateTorneoRequest req, CancellationToken ct)
    {
        var command = new CreateTorneoCommand(req.Nombre, req.Fecha, req.Lugar, req.ImagenFlyer);
        var result = await sender.Send(command, ct);
        await Send.CreatedAtAsync<GetTorneoByIdEndpoint>(new { id = result.Id }, result, cancellation: ct);
    }
}
