namespace TorneoDeportivo.Application.Features.Competidores;

/// <summary>
/// Representación de un competidor devuelta por la API hacia el cliente, con el nombre completo
/// y el nombre de la categoría ya resueltos para facilitar la visualización.
/// </summary>
public record CompetidorResponse(
    Guid Id,
    Guid TorneoId,
    Guid CategoriaId,
    string? CategoriaNombre,
    string Nombre,
    string Apellido,
    string NombreCompleto,
    int Edad,
    string Graduacion,
    decimal Peso,
    decimal Altura,
    string Escuela,
    string Responsable,
    string? Telefono);
