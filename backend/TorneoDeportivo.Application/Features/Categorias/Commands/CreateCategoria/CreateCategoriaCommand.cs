using MediatR;

namespace TorneoDeportivo.Application.Features.Categorias.Commands.CreateCategoria;

/// <summary>
/// Comando CQRS que solicita la creación de una categoría de competencia dentro de un torneo (H0002).
/// TipoCompetencia y Sexo llegan como texto y se validan/convierten a enum en el handler.
/// </summary>
public record CreateCategoriaCommand(
    Guid TorneoId,
    string Nombre,
    string TipoCompetencia,
    string Sexo,
    int RangoEdadMin,
    int RangoEdadMax,
    decimal RangoPesoMin,
    decimal RangoPesoMax,
    string RangoGraduacionMin,
    string RangoGraduacionMax) : IRequest<CategoriaResponse>;
