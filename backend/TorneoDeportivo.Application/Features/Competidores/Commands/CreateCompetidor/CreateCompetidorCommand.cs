using MediatR;

namespace TorneoDeportivo.Application.Features.Competidores.Commands.CreateCompetidor;

/// <summary>
/// Comando CQRS que solicita la carga de un competidor en un torneo (H0004). El competidor se carga sin
/// categoría; su asignación se resuelve al armar las llaves (H0005).
/// </summary>
public record CreateCompetidorCommand(
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
