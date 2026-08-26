using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

/// <summary>
/// Abstracción de acceso a datos para la entidad <see cref="Torneo"/>.
/// </summary>
public interface ITorneoRepository
{
    /// <summary>Busca un torneo por su id; devuelve null si no existe.</summary>
    Task<Torneo?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>
    /// Obtiene todos los torneos, incluidos los Finalizados.
    /// </summary>
    /// <remarks>
    /// Antes excluía los Finalizados, pero con las transiciones de H0009 eso los hacía desaparecer de la
    /// aplicación justo cuando hay que consultarlos y sacar sus reportes (H0008). El estado se muestra en
    /// la UI; filtrarlos o no es decisión de la vista, no del repositorio.
    /// </remarks>
    Task<List<Torneo>> GetAllAsync(CancellationToken ct);

    /// <summary>Persiste un nuevo torneo.</summary>
    Task AddAsync(Torneo torneo, CancellationToken ct);

    /// <summary>Actualiza un torneo existente.</summary>
    Task UpdateAsync(Torneo torneo, CancellationToken ct);

    /// <summary>Elimina un torneo.</summary>
    Task DeleteAsync(Torneo torneo, CancellationToken ct);
}
