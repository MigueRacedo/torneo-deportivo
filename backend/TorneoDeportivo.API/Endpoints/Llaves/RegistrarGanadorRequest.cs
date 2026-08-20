namespace TorneoDeportivo.API.Endpoints.Llaves;

/// <summary>
/// Solicitud para registrar el ganador de un match. <see cref="TorneoId"/> e <see cref="Id"/> (llave) se toman
/// de la ruta; <see cref="GanadorId"/> del cuerpo.
/// </summary>
public record RegistrarGanadorRequest(Guid TorneoId, Guid Id, Guid GanadorId);
