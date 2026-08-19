namespace TorneoDeportivo.API.Endpoints.Categorias;

/// <summary>
/// Parámetros de ruta para eliminar una categoría: el torneo y la categoría.
/// </summary>
public record DeleteCategoriaRequest(Guid TorneoId, Guid Id);
