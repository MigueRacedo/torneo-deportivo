using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

/// <summary>
/// Abstracción de acceso a datos para la entidad <see cref="Usuario"/>.
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>Busca un usuario por su id; devuelve null si no existe.</summary>
    Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct);

    /// <summary>Busca un usuario por su email; devuelve null si no existe.</summary>
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct);

    /// <summary>Persiste un nuevo usuario.</summary>
    Task AddAsync(Usuario usuario, CancellationToken ct);

    /// <summary>Actualiza un usuario existente.</summary>
    Task UpdateAsync(Usuario usuario, CancellationToken ct);
}
