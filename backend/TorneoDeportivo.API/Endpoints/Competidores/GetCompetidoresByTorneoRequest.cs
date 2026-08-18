namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Parámetro de ruta con el id del torneo cuyos competidores se quieren listar.
/// </summary>
public record GetCompetidoresByTorneoRequest(Guid TorneoId);
