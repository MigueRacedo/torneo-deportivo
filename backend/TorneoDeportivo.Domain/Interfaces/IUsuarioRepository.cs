using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

/// <summary>
/// Abstracción de acceso a datos para la entidad <see cref="Usuario"/>.
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>Busca un usuario por su email; devuelve null si no existe.</summary>
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct);

    /// <summary>Indica si existe al menos un usuario registrado (usado para decidir si sembrar datos iniciales).</summary>
    Task<bool> ExisteAlgunoAsync(CancellationToken ct);

    /// <summary>Persiste un nuevo usuario.</summary>
    Task AddAsync(Usuario usuario, CancellationToken ct);
}
