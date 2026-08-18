namespace TorneoDeportivo.Application.Features.Categorias;

/// <summary>
/// Campos comunes de una categoría, usados para compartir las reglas de validación entre los comandos
/// (alta y edición) y los requests de la API sin duplicarlas.
/// </summary>
public interface ICategoriaData
{
    string Nombre { get; }
    string TipoCompetencia { get; }
    string Sexo { get; }
    int? RangoEdadMin { get; }
    int? RangoEdadMax { get; }
    decimal? RangoPesoMin { get; }
    decimal? RangoPesoMax { get; }
    string RangoGraduacionMin { get; }
    string RangoGraduacionMax { get; }
}
