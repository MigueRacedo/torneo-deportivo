using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Application.Features.Llaves;

/// <summary>
/// Ayudas de mapeo de las entidades <see cref="LlaveCompetencia"/> a los DTOs del bracket, agrupando los matches
/// por ronda.
/// </summary>
public static class BracketMapping
{
    private static MatchCompetidorResponse? ToDto(Competidor? c) =>
        c is null ? null : new MatchCompetidorResponse(c.Id, $"{c.Nombre} {c.Apellido}");

    /// <summary>Convierte una llave en su representación de match.</summary>
    public static MatchResponse ToMatchResponse(this LlaveCompetencia l) => new(
        l.Id,
        l.CategoriaId,
        l.Ronda,
        l.Posicion,
        ToDto(l.Competidor1),
        ToDto(l.Competidor2),
        ToDto(l.Ganador),
        l.Estado.ToString());

    /// <summary>Agrupa las llaves de una categoría en un <see cref="BracketResponse"/> (rounds por ronda).</summary>
    public static BracketResponse ToBracketResponse(this List<LlaveCompetencia> llaves, Categoria categoria)
    {
        var totalRondas = llaves.Count == 0 ? 0 : llaves.Max(l => l.Ronda);
        var rounds = Enumerable.Range(1, totalRondas)
            .Select(ronda => llaves
                .Where(l => l.Ronda == ronda)
                .OrderBy(l => l.Posicion)
                .Select(l => l.ToMatchResponse())
                .ToList())
            .ToList();

        return new BracketResponse(categoria.Id, categoria.Nombre, totalRondas, rounds);
    }
}
