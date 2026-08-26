using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FastEndpoints;
using MediatR;
using TorneoDeportivo.Application.Features.Competidores.Queries.GetMisCompetidores;

namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Endpoint que devuelve los competidores del torneo pertenecientes a la escuela del Profesor
/// autenticado (H0007). Solo rol Profesor.
/// </summary>
/// <remarks>
/// La ruta no lleva la escuela como parámetro a propósito: se toma del claim <c>sub</c> del token, así
/// un Profesor no puede pedir los alumnos de otra escuela cambiando la URL. Es un
/// <c>EndpointWithoutRequest</c> porque el único dato de ruta es el torneo y el GET no lleva cuerpo.
/// </remarks>
public class GetMisCompetidoresEndpoint(ISender sender) : EndpointWithoutRequest<MisCompetidoresResponse>
{
    /// <summary>
    /// Configura la ruta GET /api/v1/torneos/{torneoId}/competidores/mis-alumnos, restringida al rol Profesor.
    /// </summary>
    public override void Configure()
    {
        Get("/api/v1/torneos/{torneoId}/competidores/mis-alumnos");
        Roles("Profesor");
    }

    /// <summary>
    /// Resuelve el id del usuario autenticado desde el token y envía la <see cref="GetMisCompetidoresQuery"/>.
    /// </summary>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var torneoId = Route<Guid>("torneoId");

        // El token se emite con el claim "sub", pero ASP.NET Core lo remapea a NameIdentifier porque
        // MapInboundClaims está activo (desactivarlo rompería Roles(...), que depende del mismo remapeo
        // para ClaimTypes.Role). Se leen los dos nombres para no depender de esa configuración.
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(sub, out var usuarioId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var result = await sender.Send(new GetMisCompetidoresQuery(torneoId, usuarioId), ct);
        await Send.OkAsync(result, ct);
    }
}
