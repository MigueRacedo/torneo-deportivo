using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Torneos;
using TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneos;

namespace TorneoDeportivo.API.Endpoints.Torneos;

public class GetTorneosEndpoint(ISender sender) : EndpointWithoutRequest<List<TorneoResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/torneos");
        Roles("Coordinador", "Profesor", "Director");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetTorneosQuery(), ct);
        await Send.OkAsync(result, ct);
    }
}
