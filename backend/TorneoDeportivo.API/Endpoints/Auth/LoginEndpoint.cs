using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Auth;

namespace TorneoDeportivo.API.Endpoints.Auth;

public class LoginEndpoint(ISender sender) : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/api/v1/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new LoginCommand(req.Email, req.Password), ct);
        await Send.OkAsync(result, ct);
    }
}
