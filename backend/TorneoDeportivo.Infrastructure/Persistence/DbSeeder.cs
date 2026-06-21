using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Persistence;

public static class DbSeeder
{
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
