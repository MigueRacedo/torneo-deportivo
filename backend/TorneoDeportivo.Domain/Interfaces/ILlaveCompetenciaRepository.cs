using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

/// <summary>
/// Abstracción de acceso a datos para la entidad <see cref="LlaveCompetencia"/> (nodos del bracket).
/// </summary>
public interface ILlaveCompetenciaRepository
{
    /// <summary>Obtiene todas las llaves (nodos del bracket) de una categoría.</summary>
    Task<List<LlaveCompetencia>> GetByCategoriaIdAsync(Guid categoriaId, CancellationToken ct);

    /// <summary>Busca una llave por su id; devuelve null si no existe.</summary>
    Task<LlaveCompetencia?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Persiste en bloque el conjunto de llaves generado para un bracket.</summary>
    Task AddRangeAsync(IEnumerable<LlaveCompetencia> llaves, CancellationToken ct);

    /// <summary>Actualiza una llave existente (e.g. al registrar un ganador).</summary>
    Task UpdateAsync(LlaveCompetencia llave, CancellationToken ct);
}
