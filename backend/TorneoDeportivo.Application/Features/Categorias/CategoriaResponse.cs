namespace TorneoDeportivo.Application.Features.Categorias;

/// <summary>
/// Representación de una categoría de competencia devuelta por la API hacia el cliente.
/// Los enums de dominio (TipoCompetencia, Sexo) se exponen como texto para el frontend.
/// </summary>
/// <remarks>
/// <see cref="TotalCompetidores"/> es nullable a propósito: solo el listado de categorías lo calcula
/// (H0006, consulta del Profesor). En las respuestas de crear/editar llega en null, que significa
/// "no calculado" y se distingue de un 0 real ("la categoría no tiene competidores").
/// </remarks>
public record CategoriaResponse(
    Guid Id,
    Guid TorneoId,
    string Nombre,
    string TipoCompetencia,
    string Sexo,
    int? RangoEdadMin,
    int? RangoEdadMax,
    decimal? RangoPesoMin,
    decimal? RangoPesoMax,
    string RangoGraduacionMin,
    string RangoGraduacionMax,
    bool LlavesGeneradas,
    int? TotalCompetidores = null);
