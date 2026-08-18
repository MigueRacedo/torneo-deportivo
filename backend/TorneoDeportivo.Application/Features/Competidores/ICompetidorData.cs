namespace TorneoDeportivo.Application.Features.Competidores;

/// <summary>
/// Campos comunes de un competidor, usados para compartir las reglas de validación entre el comando de alta
/// y el request de la API sin duplicarlas.
/// </summary>
public interface ICompetidorData
{
    string Nombre { get; }
    string Apellido { get; }
    int Edad { get; }
    string Graduacion { get; }
    decimal Peso { get; }
    decimal Altura { get; }
    string Escuela { get; }
    string Responsable { get; }
    string? Telefono { get; }
}
