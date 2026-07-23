using System.Security.Cryptography;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Services;

/// <summary>
/// Implementación de <see cref="IPasswordHasher"/> basada en PBKDF2 (Rfc2898DeriveBytes) con salt aleatorio
/// por contraseña y comparación en tiempo constante.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <summary>
    /// Genera un salt aleatorio y deriva la clave PBKDF2 de la contraseña, devolviendo "salt.key" en Base64.
    /// </summary>
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    /// <summary>
    /// Recalcula la clave derivada usando el salt almacenado en el hash y la compara en tiempo constante
    /// con la clave esperada.
    /// </summary>
    public bool Verify(string password, string hash)
    {
        var parts = hash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedKey = Convert.FromBase64String(parts[1]);
        var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return CryptographicOperations.FixedTimeEquals(expectedKey, actualKey);
    }
}
