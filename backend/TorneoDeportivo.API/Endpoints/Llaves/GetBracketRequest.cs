namespace TorneoDeportivo.API.Endpoints.Llaves;

/// <summary>
/// Parámetros de ruta para obtener el bracket de una categoría dentro de un torneo.
/// </summary>
public record GetBracketRequest(Guid TorneoId, Guid CategoriaId);
