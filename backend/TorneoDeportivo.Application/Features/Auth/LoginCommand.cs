using MediatR;

namespace TorneoDeportivo.Application.Features.Auth;

/// <summary>
/// Comando CQRS que solicita autenticar a un usuario con email y contraseña.
/// </summary>
public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
