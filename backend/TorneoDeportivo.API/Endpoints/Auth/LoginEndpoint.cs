using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Auth;

namespace TorneoDeportivo.API.Endpoints.Auth;

/// <summary>
/// Endpoint público de autenticación: recibe credenciales, delega el login en el CQRS y devuelve el token JWT.
/// </summary>
public class LoginEndpoint(ISender sender) : Endpoint<LoginRequest, LoginResponse>
{
    /// <summary>
    /// Configura la ruta POST /api/v1/auth/login como anónima (no requiere autenticación previa).
    /// </summary>
    public override void Configure()
    {
        Post("/api/v1/auth/login");
        AllowAnonymous();
    }

    /// <summary>
    /// Procesa la solicitud de login enviando el <see cref="LoginCommand"/> correspondiente y devuelve el resultado.
    /// </summary>
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await sender.Send(new LoginCommand(req.Email, req.Password), ct);
        await Send.OkAsync(result, ct);
    }
}
