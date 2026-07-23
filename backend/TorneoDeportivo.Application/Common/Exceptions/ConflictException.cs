namespace TorneoDeportivo.Application.Common.Exceptions;

/// <summary>
/// Excepción que representa un conflicto de negocio (e.g. torneo que ya tiene llaves generadas).
/// El middleware global la traduce a un HTTP 409.
/// </summary>
public class ConflictException(string message) : Exception(message);
