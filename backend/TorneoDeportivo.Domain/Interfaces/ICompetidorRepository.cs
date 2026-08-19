using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

/// <summary>
/// Abstracción de acceso a datos para la entidad <see cref="Competidor"/>.
/// </summary>
public interface ICompetidorRepository
{
    /// <summary>Busca un competidor por su id; devuelve null si no existe.</summary>
    Task<Competidor?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Obtiene todos los competidores inscriptos en un torneo.</summary>
    Task<List<Competidor>> GetByTorneoIdAsync(Guid torneoId, CancellationToken ct);

    /// <summary>Obtiene los competidores de un torneo pertenecientes a una escuela específica (vista del Profesor).</summary>
    Task<List<Competidor>> GetByEscuelaAsync(Guid torneoId, string escuela, CancellationToken ct);

    /// <summary>Persiste un nuevo competidor.</summary>
    Task AddAsync(Competidor competidor, CancellationToken ct);

    /// <summary>Actualiza un competidor existente.</summary>
    Task UpdateAsync(Competidor competidor, CancellationToken ct);
}
