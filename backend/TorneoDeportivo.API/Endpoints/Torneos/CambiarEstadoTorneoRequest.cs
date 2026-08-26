namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Pedido de transición de estado de un torneo: el id va en la ruta y el estado destino en el cuerpo.
/// </summary>
public record CambiarEstadoTorneoRequest(Guid Id, string Estado, bool Forzar = false);
