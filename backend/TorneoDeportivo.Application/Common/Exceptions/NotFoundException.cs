namespace TorneoDeportivo.Application.Common.Exceptions;

public class NotFoundException(string entidad, object id)
    : Exception($"{entidad} con id '{id}' no fue encontrado.");
