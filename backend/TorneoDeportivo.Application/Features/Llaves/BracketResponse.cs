namespace TorneoDeportivo.Application.Features.Llaves;

/// <summary>Competidor tal como se muestra dentro de un match del bracket (datos mínimos).</summary>
public record MatchCompetidorResponse(Guid Id, string NombreCompleto);

/// <summary>Un enfrentamiento del bracket con sus dos competidores (o bye), su ganador y su estado.</summary>
public record MatchResponse(
    Guid Id,
    Guid CategoriaId,
    int Ronda,
    int Posicion,
    MatchCompetidorResponse? Competidor1,
    MatchCompetidorResponse? Competidor2,
    MatchCompetidorResponse? Ganador,
    string Estado);

/// <summary>
/// Bracket completo de una categoría: metadatos y los matches agrupados por ronda (rounds[ronda][match]).
/// </summary>
public record BracketResponse(
    Guid CategoriaId,
    string CategoriaNombre,
    int TotalRondas,
    List<List<MatchResponse>> Rounds);
