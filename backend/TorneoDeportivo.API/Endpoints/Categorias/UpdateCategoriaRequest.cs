using TorneoDeportivo.Application.Features.Categorias;

namespace TorneoDeportivo.API.Endpoints.Categorias;

/// <summary>
/// Cuerpo de la solicitud HTTP para editar una categoría. <see cref="TorneoId"/> e <see cref="Id"/> se toman
/// de la ruta; el resto de los campos, del cuerpo JSON.
/// </summary>
public record UpdateCategoriaRequest(
    Guid TorneoId,
    Guid Id,
    string Nombre,
    string TipoCompetencia,
    string Sexo,
    int? RangoEdadMin,
    int? RangoEdadMax,
    decimal? RangoPesoMin,
    decimal? RangoPesoMax,
    string RangoGraduacionMin,
    string RangoGraduacionMax) : ICategoriaData;
