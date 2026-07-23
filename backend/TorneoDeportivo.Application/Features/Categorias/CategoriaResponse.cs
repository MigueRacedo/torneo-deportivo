namespace TorneoDeportivo.Application.Features.Categorias;

/// <summary>
/// Representación de una categoría de competencia devuelta por la API hacia el cliente.
/// Los enums de dominio (TipoCompetencia, Sexo) se exponen como texto para el frontend.
/// </summary>
public record CategoriaResponse(
    Guid Id,
    Guid TorneoId,
    string Nombre,
    string TipoCompetencia,
    string Sexo,
    int RangoEdadMin,
    int RangoEdadMax,
    decimal RangoPesoMin,
    decimal RangoPesoMax,
    string RangoGraduacionMin,
    string RangoGraduacionMax);
