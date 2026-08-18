using MediatR;

namespace TorneoDeportivo.Application.Features.Categorias.Commands.UpdateCategoria;

/// <summary>
/// Comando CQRS que solicita la edición de una categoría existente de un torneo.
/// TipoCompetencia y Sexo llegan como texto y se validan/convierten a enum en el handler.
/// </summary>
public record UpdateCategoriaCommand(
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
    string RangoGraduacionMax) : IRequest<CategoriaResponse>, ICategoriaData;
