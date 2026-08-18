using MediatR;

namespace TorneoDeportivo.Application.Features.Competidores.Commands.CreateCompetidor;

/// <summary>
/// Comando CQRS que solicita la carga de un competidor en una categoría de un torneo (H0004).
/// </summary>
public record CreateCompetidorCommand(
    Guid TorneoId,
    Guid CategoriaId,
    string Nombre,
    string Apellido,
    int Edad,
    string Graduacion,
    decimal Peso,
    decimal Altura,
    string Escuela,
    string Responsable,
    string? Telefono) : IRequest<CompetidorResponse>, ICompetidorData;
