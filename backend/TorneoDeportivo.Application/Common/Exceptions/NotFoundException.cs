namespace TorneoDeportivo.Application.Common.Exceptions;

/// <summary>
/// Excepción que indica que una entidad solicitada no existe. El middleware global la traduce a un HTTP 404.
/// </summary>
public class NotFoundException(string entidad, object id)
    : Exception($"{entidad} con id '{id}' no fue encontrado.");
