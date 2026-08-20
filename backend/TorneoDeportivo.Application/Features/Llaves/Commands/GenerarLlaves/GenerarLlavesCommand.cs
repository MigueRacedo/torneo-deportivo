using MediatR;

namespace TorneoDeportivo.Application.Features.Llaves.Commands.GenerarLlaves;

/// <summary>
/// Comando CQRS que clasifica los competidores del torneo en sus categorías y genera las llaves de cada
/// categoría con al menos 2 competidores (H0005 ⭐).
/// </summary>
public record GenerarLlavesCommand(Guid TorneoId) : IRequest<GenerarLlavesResponse>;

/// <summary>Resultado de la generación de llaves para una categoría del torneo.</summary>
public record GenerarLlavesResultado(
    Guid CategoriaId,
    string CategoriaNombre,
    int CompetidoresClasificados,
    bool LlavesGeneradas,
    string? Motivo);

/// <summary>Resumen de la generación de llaves del torneo: competidores sin clasificar y resultado por categoría.</summary>
public record GenerarLlavesResponse(int CompetidoresSinClasificar, List<GenerarLlavesResultado> Categorias);
