using NSubstitute;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using TorneoDeportivo.Infrastructure.Persistence;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Auth;

/// <summary>
/// Cubre el sembrado de usuarios demo. El comportamiento importa: el seeder corre en cada arranque de la
/// API, así que tiene que ser idempotente y, a la vez, capaz de agregar los roles que se sumen después.
/// </summary>
public class DbSeederTests
{
    private const string EmailCoordinador = "coordinador@torneo.test";
    private const string EmailProfesor = "profesor@torneo.test";

    private static (IUsuarioRepository repo, IPasswordHasher hasher) Deps(params string[] emailsExistentes)
    {
        var repo = Substitute.For<IUsuarioRepository>();
        // Por defecto no existe nadie; los emails indicados se devuelven como ya registrados.
        repo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Usuario?)null);
        foreach (var email in emailsExistentes)
        {
            repo.GetByEmailAsync(email, Arg.Any<CancellationToken>())
                .Returns(new Usuario { Id = Guid.NewGuid(), Email = email });
        }

        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash(Arg.Any<string>()).Returns(call => $"hash::{call.Arg<string>()}");
        return (repo, hasher);
    }

    [Fact]
    public async Task SeedAsync_BaseVacia_CreaUnUsuarioPorRolDemo()
    {
        var (repo, hasher) = Deps();

        await DbSeeder.SeedAsync(repo, hasher);

        await repo.Received(1).AddAsync(Arg.Is<Usuario>(u => u.Rol == RolUsuario.Coordinador), Arg.Any<CancellationToken>());
        await repo.Received(1).AddAsync(Arg.Is<Usuario>(u => u.Rol == RolUsuario.Profesor), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SeedAsync_UsuariosYaExisten_NoCreaNinguno()
    {
        // Idempotencia: el seeder corre en cada arranque y no debe duplicar cuentas.
        var (repo, hasher) = Deps(EmailCoordinador, EmailProfesor);

        await DbSeeder.SeedAsync(repo, hasher);

        await repo.DidNotReceive().AddAsync(Arg.Any<Usuario>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SeedAsync_BaseConSoloElCoordinador_CreaSoloElRolFaltante()
    {
        // Este es el caso que motivó el cambio en H0006: una base que ya venía con el Coordinador debe
        // recibir el Profesor al reiniciar, sin tener que recrear la base.
        var (repo, hasher) = Deps(EmailCoordinador);

        await DbSeeder.SeedAsync(repo, hasher);

        await repo.Received(1).AddAsync(Arg.Is<Usuario>(u => u.Email == EmailProfesor), Arg.Any<CancellationToken>());
        await repo.DidNotReceive().AddAsync(Arg.Is<Usuario>(u => u.Email == EmailCoordinador), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SeedAsync_PersisteElHashYNoLaContrasenaEnClaro()
    {
        var repo = Substitute.For<IUsuarioRepository>();
        repo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Usuario?)null);
        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Hash(Arg.Any<string>()).Returns("HASH-OPACO");

        await DbSeeder.SeedAsync(repo, hasher);

        // Lo que importa es que la contraseña pase por el hasher y que se persista su salida.
        await repo.Received(2).AddAsync(
            Arg.Is<Usuario>(u => u.PasswordHash == "HASH-OPACO"), Arg.Any<CancellationToken>());
        hasher.Received(2).Hash(Arg.Any<string>());
    }
}
