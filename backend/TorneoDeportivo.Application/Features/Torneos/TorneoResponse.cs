namespace TorneoDeportivo.Application.Features.Torneos;

/// <summary>
/// Representación de un torneo devuelta por la API hacia el cliente.
/// </summary>
public record TorneoResponse(
    Guid Id,
    string Nombre,
    DateOnly Fecha,
    string Lugar,
    string? ImagenFlyer,
    string Estado);
