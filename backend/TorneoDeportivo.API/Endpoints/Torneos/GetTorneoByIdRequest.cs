namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Parámetro de ruta con el id del torneo a consultar.
/// </summary>
public record GetTorneoByIdRequest(Guid Id);
