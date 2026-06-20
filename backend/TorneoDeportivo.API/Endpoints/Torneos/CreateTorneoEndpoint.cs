using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Torneos;
using TorneoDeportivo.Application.Features.Torneos.Commands.CreateTorneo;

namespace TorneoDeportivo.API.Endpoints.Torneos;

public class CreateTorneoEndpoint(ISender sender) : Endpoint<CreateTorneoRequest, TorneoResponse>
{
    public override void Configure()
    {
        Post("/api/v1/torneos");
        Roles("Coordinador");
        Description(b => b.Produces<TorneoResponse>(201));
    }

    public override async Task HandleAsync(CreateTorneoRequest req, CancellationToken ct)
    {
        var command = new CreateTorneoCommand(req.Nombre, req.Fecha, req.Lugar, req.ImagenFlyer);
        var result = await sender.Send(command, ct);
        await Send.CreatedAtAsync<GetTorneoByIdEndpoint>(new { id = result.Id }, result, cancellation: ct);
    }
}
