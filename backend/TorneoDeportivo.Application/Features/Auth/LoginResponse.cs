namespace TorneoDeportivo.Application.Features.Auth;

/// <summary>
/// Resultado de un login exitoso: el token JWT y los datos básicos del usuario autenticado.
/// </summary>
public record LoginResponse(string Token, string Nombre, string Email, string Rol);
