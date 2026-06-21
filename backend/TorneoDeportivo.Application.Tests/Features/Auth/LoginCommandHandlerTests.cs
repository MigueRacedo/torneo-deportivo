using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Common.Interfaces;
using TorneoDeportivo.Application.Features.Auth;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Auth;

public class LoginCommandHandlerTests
{
    private static Usuario CrearUsuario() => new()
    {
        Id = Guid.NewGuid(),
        Nombre = "Coordinador Demo",
        Email = "coordinador@torneo.test",
        PasswordHash = "hash-guardado",
        Rol = RolUsuario.Coordinador
    };

    [Fact]
    public async Task Handle_CredencialesValidas_ReturnsLoginResponseConToken()
    {
        var usuario = CrearUsuario();
        var usuarioRepo = Substitute.For<IUsuarioRepository>();
        usuarioRepo.GetByEmailAsync(usuario.Email, Arg.Any<CancellationToken>()).Returns(usuario);

        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Verify("password-correcta", usuario.PasswordHash).Returns(true);

        var jwt = Substitute.For<IJwtTokenGenerator>();
        jwt.GenerateToken(usuario).Returns("token-generado");

        var handler = new LoginCommandHandler(usuarioRepo, hasher, jwt);

        var result = await handler.Handle(new LoginCommand(usuario.Email, "password-correcta"), CancellationToken.None);

        Assert.Equal("token-generado", result.Token);
        Assert.Equal(usuario.Nombre, result.Nombre);
        Assert.Equal("Coordinador", result.Rol);
    }

    [Fact]
    public async Task Handle_UsuarioNoExiste_LanzaUnauthorizedException()
    {
        var usuarioRepo = Substitute.For<IUsuarioRepository>();
        usuarioRepo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Usuario?)null);

        var handler = new LoginCommandHandler(usuarioRepo, Substitute.For<IPasswordHasher>(), Substitute.For<IJwtTokenGenerator>());

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => handler.Handle(new LoginCommand("nadie@torneo.test", "cualquiera"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PasswordIncorrecta_LanzaUnauthorizedException()
    {
        var usuario = CrearUsuario();
        var usuarioRepo = Substitute.For<IUsuarioRepository>();
        usuarioRepo.GetByEmailAsync(usuario.Email, Arg.Any<CancellationToken>()).Returns(usuario);

        var hasher = Substitute.For<IPasswordHasher>();
        hasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var handler = new LoginCommandHandler(usuarioRepo, hasher, Substitute.For<IJwtTokenGenerator>());

        await Assert.ThrowsAsync<UnauthorizedException>(
            () => handler.Handle(new LoginCommand(usuario.Email, "password-incorrecta"), CancellationToken.None));
    }
}
