namespace TorneoDeportivo.API.Endpoints.Categorias;

/// <summary>
/// Parámetro de ruta con el id del torneo cuyas categorías se quieren listar.
/// </summary>
public record GetCategoriasByTorneoRequest(Guid TorneoId);
