namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Cuerpo de la solicitud HTTP para editar un torneo. <see cref="Id"/> se toma de la ruta.
/// </summary>
public record UpdateTorneoRequest(Guid Id, string Nombre, DateOnly Fecha, string Lugar, string? ImagenFlyer);
