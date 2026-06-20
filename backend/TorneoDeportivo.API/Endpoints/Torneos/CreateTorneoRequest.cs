namespace TorneoDeportivo.API.Endpoints.Torneos;

public record CreateTorneoRequest(string Nombre, DateOnly Fecha, string Lugar, string? ImagenFlyer);
