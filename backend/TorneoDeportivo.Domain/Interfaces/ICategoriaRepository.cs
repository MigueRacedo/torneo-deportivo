using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

/// <summary>
/// Abstracción de acceso a datos para la entidad <see cref="Categoria"/>.
/// </summary>
public interface ICategoriaRepository
{
    /// <summary>Busca una categoría por su id; devuelve null si no existe.</summary>
    Task<Categoria?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Obtiene todas las categorías definidas para un torneo.</summary>
    Task<List<Categoria>> GetByTorneoIdAsync(Guid torneoId, CancellationToken ct);

    /// <summary>Persiste una nueva categoría.</summary>
    Task AddAsync(Categoria categoria, CancellationToken ct);

    /// <summary>Actualiza una categoría existente.</summary>
    Task UpdateAsync(Categoria categoria, CancellationToken ct);
}
