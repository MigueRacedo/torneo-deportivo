using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Persistence;

/// <summary>
/// Siembra los usuarios demo de la base de datos al arrancar la aplicación (uno por rol).
/// </summary>
public static class DbSeeder
{
    /// <summary>Usuario demo a sembrar: credenciales y rol.</summary>
    private record UsuarioSemilla(string Nombre, string Email, string Password, RolUsuario Rol, string? Escuela = null);

    private static readonly UsuarioSemilla[] Semillas =
    [
        new("Coordinador Demo", "coordinador@torneo.test", "Coordinador123!", RolUsuario.Coordinador),
        new("Profesor Demo", "profesor@torneo.test", "Profesor123!", RolUsuario.Profesor, "Escuela Central"),
    ];

    /// <summary>
    /// Crea los usuarios demo que todavía no existan, verificando <b>uno por uno por email</b>.
    /// </summary>
    /// <remarks>
    /// La verificación es por email y no "¿hay algún usuario?" a propósito: con la comprobación global,
    /// una base que ya tenía al Coordinador nunca habría recibido a los usuarios de los roles agregados
    /// después (el Profesor de H0006), y la única forma de obtenerlos sería recrear la base.
    /// Así el seeder es idempotente y además incremental.
    /// </remarks>
    public static async Task SeedAsync(IUsuarioRepository usuarios, IPasswordHasher passwordHasher, CancellationToken ct = default)
    {
        foreach (var semilla in Semillas)
        {
            if (await usuarios.GetByEmailAsync(semilla.Email, ct) is not null)
                continue;

            await usuarios.AddAsync(new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = semilla.Nombre,
                Email = semilla.Email,
                PasswordHash = passwordHasher.Hash(semilla.Password),
                Rol = semilla.Rol,
                Escuela = semilla.Escuela
            }, ct);
        }
    }
}
