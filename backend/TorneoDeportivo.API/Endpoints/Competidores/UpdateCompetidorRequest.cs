using TorneoDeportivo.Application.Features.Competidores;

namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Cuerpo de la solicitud HTTP para editar un competidor. <see cref="TorneoId"/> e <see cref="Id"/> se toman
/// de la ruta; el resto de los campos, del cuerpo JSON.
/// </summary>
public record UpdateCompetidorRequest(
    Guid TorneoId,
    Guid Id,
    string Nombre,
    string Apellido,
    string Sexo,
    int Edad,
    string Graduacion,
    decimal Peso,
    decimal Altura,
    string Escuela,
    string Responsable,
    string? Telefono) : ICompetidorData;
