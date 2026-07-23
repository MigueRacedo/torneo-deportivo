namespace TorneoDeportivo.API.Endpoints.Categorias;

/// <summary>
/// Cuerpo de la solicitud HTTP para crear una categoría. <see cref="TorneoId"/> se toma de la ruta;
/// el resto de los campos, del cuerpo JSON.
/// </summary>
public record CreateCategoriaRequest(
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
