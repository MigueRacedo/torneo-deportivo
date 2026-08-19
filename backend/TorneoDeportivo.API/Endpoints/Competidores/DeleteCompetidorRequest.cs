namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Parámetros de ruta para eliminar un competidor: el torneo y el competidor.
/// </summary>
public record DeleteCompetidorRequest(Guid TorneoId, Guid Id);
