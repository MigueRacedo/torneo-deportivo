namespace TorneoDeportivo.API.Endpoints.Auth;

/// <summary>
/// Cuerpo de la solicitud HTTP de login con las credenciales del usuario.
/// </summary>
public record LoginRequest(string Email, string Password);
