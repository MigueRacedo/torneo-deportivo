using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Application.Features.Llaves;

/// <summary>
/// Genera el bracket de eliminación directa (single-elimination) de una categoría a partir de sus competidores:
/// calcula el tamaño de bracket (siguiente potencia de 2), mezcla los competidores, distribuye los BYEs de forma
/// uniforme en la primera ronda, crea los matches vacíos de las rondas siguientes y propaga los ganadores de los BYEs.
/// Ver <c>skills/bracket-algorithm.md</c>.
/// </summary>
public static class BracketGeneratorService
{
    /// <summary>
    /// Construye la lista de <see cref="LlaveCompetencia"/> del bracket de la categoría. Con 1 competidor devuelve
    /// una lista vacía (campeón directo, sin enfrentamientos); con 0 lanza una excepción.
    /// </summary>
    public static List<LlaveCompetencia> Generar(Guid categoriaId, IReadOnlyList<Competidor> competidores)
    {
        var n = competidores.Count;
        if (n == 0)
            throw new InvalidOperationException("Se necesita al menos un competidor para generar las llaves.");

        // Caso especial: un único competidor es campeón directo, no hay matches.
        if (n == 1)
            return [];

        var mezclados = competidores.OrderBy(_ => Random.Shared.Next()).ToList();

        var tamañoBracket = 1;
        while (tamañoBracket < n) tamañoBracket *= 2;
        var totalRondas = (int)Math.Log2(tamañoBracket);
        var matchesRonda1 = tamañoBracket / 2;
        var byes = tamañoBracket - n;

        // Distribuir los BYEs de forma uniforme entre los matches de la ronda 1 (para que no queden juntos).
        var esBye = new bool[matchesRonda1];
        for (var k = 0; k < byes; k++)
            esBye[(int)((long)k * matchesRonda1 / byes)] = true;

        var llaves = new List<LlaveCompetencia>();

        // Ronda 1: emparejar competidores, dejando un BYE (rival null) donde corresponde.
        var ci = 0;
        for (var pos = 0; pos < matchesRonda1; pos++)
        {
            var llave = new LlaveCompetencia
            {
                Id = Guid.NewGuid(),
                CategoriaId = categoriaId,
                Ronda = 1,
                Posicion = pos,
            };

            if (esBye[pos])
            {
                llave.Competidor1Id = mezclados[ci++].Id;
                llave.Competidor2Id = null;
                llave.GanadorId = llave.Competidor1Id;
                llave.Estado = EstadoLlave.Bye;
            }
            else
            {
                llave.Competidor1Id = mezclados[ci++].Id;
                llave.Competidor2Id = mezclados[ci++].Id;
                llave.Estado = EstadoLlave.Pendiente;
            }

            llaves.Add(llave);
        }

        // Rondas siguientes: matches vacíos (se completan al avanzar los ganadores).
        var matchesPorRonda = matchesRonda1;
        for (var ronda = 2; ronda <= totalRondas; ronda++)
        {
            matchesPorRonda /= 2;
            for (var pos = 0; pos < matchesPorRonda; pos++)
            {
                llaves.Add(new LlaveCompetencia
                {
                    Id = Guid.NewGuid(),
                    CategoriaId = categoriaId,
                    Ronda = ronda,
                    Posicion = pos,
                    Estado = EstadoLlave.Pendiente,
                });
            }
        }

        // Propagar los ganadores de los BYEs a la ronda siguiente.
        foreach (var bye in llaves.Where(l => l.Estado == EstadoLlave.Bye).ToList())
            Propagar(bye.GanadorId, bye.Ronda, bye.Posicion, llaves);

        return llaves;
    }

    /// <summary>
    /// Coloca al ganador de un match en el slot correspondiente del match de la ronda siguiente.
    /// </summary>
    public static void Propagar(Guid? ganadorId, int ronda, int posicion, List<LlaveCompetencia> todas)
    {
        var siguiente = todas.FirstOrDefault(l => l.Ronda == ronda + 1 && l.Posicion == posicion / 2);
        if (siguiente is null) return;

        if (posicion % 2 == 0) siguiente.Competidor1Id = ganadorId;
        else siguiente.Competidor2Id = ganadorId;
    }
}
