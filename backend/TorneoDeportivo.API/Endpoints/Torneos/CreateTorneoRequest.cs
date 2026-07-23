namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Cuerpo de la solicitud HTTP para crear un torneo nuevo.
/// </summary>
public record CreateTorneoRequest(string Nombre, DateOnly Fecha, string Lugar, string? ImagenFlyer);
