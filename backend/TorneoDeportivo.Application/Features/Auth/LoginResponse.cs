namespace TorneoDeportivo.Application.Features.Auth;

public record LoginResponse(string Token, string Nombre, string Email, string Rol);
