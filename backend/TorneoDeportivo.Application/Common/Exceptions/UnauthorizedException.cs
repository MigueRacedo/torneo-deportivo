namespace TorneoDeportivo.Application.Common.Exceptions;

/// <summary>
/// Excepción que indica una falla de autenticación (e.g. credenciales inválidas).
/// El middleware global la traduce a un HTTP 401.
/// </summary>
public class UnauthorizedException(string message) : Exception(message);
