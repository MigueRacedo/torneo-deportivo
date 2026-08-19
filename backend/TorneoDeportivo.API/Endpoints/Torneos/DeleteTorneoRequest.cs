namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Parámetro de ruta con el id del torneo a eliminar.
/// </summary>
public record DeleteTorneoRequest(Guid Id);
