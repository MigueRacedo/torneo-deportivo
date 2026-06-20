using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Torneos;
using TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneoById;

namespace TorneoDeportivo.API.Endpoints.Torneos;

public class GetTorneoByIdEndpoint(ISender sender) : Endpoint<GetTorneoByIdRequest, TorneoResponse>
{
    public override void Configure()
    {
        Get("/api/v1/torneos/{id}");
        Roles("Coordinador", "Profesor", "Director");
    }

    public override async Task HandleAsync(GetTorneoByIdRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new GetTorneoByIdQuery(req.Id), ct);
        await Send.OkAsync(result, ct);
    }
}
