using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

/// <summary>
/// Abstracción de acceso a datos para la entidad <see cref="Torneo"/>.
/// </summary>
public interface ITorneoRepository
{
    /// <summary>Busca un torneo por su id; devuelve null si no existe.</summary>
    Task<Torneo?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Obtiene todos los torneos que no están en estado Finalizado.</summary>
    Task<List<Torneo>> GetAllActivosAsync(CancellationToken ct);

    /// <summary>Persiste un nuevo torneo.</summary>
    Task AddAsync(Torneo torneo, CancellationToken ct);

    /// <summary>Actualiza un torneo existente.</summary>
    Task UpdateAsync(Torneo torneo, CancellationToken ct);

    /// <summary>Elimina un torneo.</summary>
    Task DeleteAsync(Torneo torneo, CancellationToken ct);
}
