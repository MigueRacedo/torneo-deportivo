using TorneoDeportivo.Application.Features.Competidores;

namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Cuerpo de la solicitud HTTP para cargar un competidor. <see cref="TorneoId"/> se toma de la ruta;
/// el resto de los campos, del cuerpo JSON.
/// </summary>
public record CreateCompetidorRequest(
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
    string? Telefono) : ICompetidorData;
