using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Application.Common.Interfaces;

/// <summary>
/// Abstracción para la generación de tokens JWT a partir de un usuario autenticado.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Genera un token JWT firmado que representa la identidad y el rol del usuario dado.
    /// </summary>
    string GenerateToken(Usuario usuario);
}
