namespace TorneoDeportivo.Application.Features.Torneos;

public record TorneoResponse(
    Guid Id,
    string Nombre,
    DateOnly Fecha,
    string Lugar,
    string? ImagenFlyer,
    string Estado);
