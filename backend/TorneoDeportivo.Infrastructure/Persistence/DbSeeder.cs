using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Persistence;

/// <summary>
/// Siembra datos iniciales de la base de datos al arrancar la aplicación (usuario Coordinador demo).
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Crea un usuario Coordinador de prueba si todavía no existe ningún usuario registrado; no hace nada
    /// si la base de datos ya tiene usuarios.
    /// </summary>
    public static async Task SeedAsync(IUsuarioRepository usuarios, IPasswordHasher passwordHasher, CancellationToken ct = default)
    {
        if (await usuarios.ExisteAlgunoAsync(ct))
            return;

        var coordinador = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Coordinador Demo",
            Email = "coordinador@torneo.test",
            PasswordHash = passwordHasher.Hash("Coordinador123!"),
            Rol = RolUsuario.Coordinador
        };

        await usuarios.AddAsync(coordinador, ct);
    }
}
