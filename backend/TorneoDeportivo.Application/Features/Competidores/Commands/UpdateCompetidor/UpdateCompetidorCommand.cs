using MediatR;

namespace TorneoDeportivo.Application.Features.Competidores.Commands.UpdateCompetidor;

/// <summary>
/// Comando CQRS que solicita la edición de los datos de un competidor de un torneo. No cambia su categoría
/// (esa asignación se resuelve al armar las llaves).
/// </summary>
public record UpdateCompetidorCommand(
    Guid Id,
    Guid TorneoId,
    string Nombre,
    string Apellido,
    string Sexo,
    int Edad,
    string Graduacion,
    decimal Peso,
    decimal Altura,
    string Escuela,
    string Responsable,
    string? Telefono) : IRequest<CompetidorResponse>, ICompetidorData;
