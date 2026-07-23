using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Common.Interfaces;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Auth;

/// <summary>
/// Handler CQRS que procesa el comando de login: busca el usuario por email, verifica la contraseña
/// y genera el token JWT correspondiente.
/// </summary>
public class LoginCommandHandler(
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<LoginCommand, LoginResponse>
{
    private const string CredencialesInvalidas = "Email o contraseña incorrectos.";

    /// <summary>
    /// Valida las credenciales del usuario y, si son correctas, devuelve un <see cref="LoginResponse"/>
    /// con el token JWT generado; en caso contrario lanza <see cref="UnauthorizedException"/>.
    /// </summary>
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var usuario = await usuarioRepository.GetByEmailAsync(request.Email, ct);
        var passwordValida = usuario is not null && passwordHasher.Verify(request.Password, usuario.PasswordHash);

        if (!passwordValida)
        {
            // Si el usuario no existe, igual se calcula un hash (mismo costo que
            // una verificación real) para no filtrar por timing qué emails existen.
            if (usuario is null) passwordHasher.Hash(request.Password);
            throw new UnauthorizedException(CredencialesInvalidas);
        }

        var token = jwtTokenGenerator.GenerateToken(usuario);

        return new LoginResponse(token, usuario.Nombre, usuario.Email, usuario.Rol.ToString());
    }
}
