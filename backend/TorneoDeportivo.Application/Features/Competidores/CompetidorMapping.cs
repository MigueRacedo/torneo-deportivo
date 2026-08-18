using Mapster;
using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Application.Features.Competidores;

/// <summary>
/// Ayudas de mapeo de <see cref="Competidor"/> a <see cref="CompetidorResponse"/>. Usa Mapster para los campos
/// directos y completa los calculados (nombre completo y nombre de la categoría) que no existen en la entidad.
/// </summary>
public static class CompetidorMapping
{
    /// <summary>
    /// Convierte un competidor en su respuesta, completando el nombre completo y el nombre de la categoría
    /// (este último tomado del parámetro o de la navegación cargada).
    /// </summary>
    public static CompetidorResponse ToResponse(this Competidor c, string? categoriaNombre = null) =>
        c.Adapt<CompetidorResponse>() with
        {
            NombreCompleto = $"{c.Nombre} {c.Apellido}",
            CategoriaNombre = categoriaNombre ?? c.Categoria?.Nombre,
        };
}
