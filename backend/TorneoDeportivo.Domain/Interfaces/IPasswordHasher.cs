namespace TorneoDeportivo.Domain.Interfaces;

/// <summary>
/// Abstracción para el hashing y verificación de contraseñas de usuarios.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Genera el hash seguro de una contraseña en texto plano.</summary>
    string Hash(string password);

    /// <summary>Verifica si una contraseña en texto plano corresponde al hash almacenado.</summary>
    bool Verify(string password, string hash);
}
