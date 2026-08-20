using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Application.Features.Llaves;

/// <summary>
/// Determina si un competidor "encaja" en una categoría comparando sus atributos (sexo, edad, peso y graduación)
/// contra los rangos de la categoría. Se usa al armar las llaves (H0005) para clasificar a los competidores.
/// </summary>
public static class ClasificadorCompetidores
{
    /// <summary>
    /// Devuelve true si el competidor cumple todos los criterios de la categoría: mismo sexo, edad dentro del
    /// rango (abierto si algún extremo es null), peso dentro del rango (si la categoría lo define) y graduación
    /// dentro del rango.
    /// </summary>
    public static bool Encaja(Competidor competidor, Categoria categoria)
    {
        if (competidor.Sexo != categoria.Sexo)
            return false;

        if (categoria.RangoEdadMin.HasValue && competidor.Edad < categoria.RangoEdadMin.Value)
            return false;
        if (categoria.RangoEdadMax.HasValue && competidor.Edad > categoria.RangoEdadMax.Value)
            return false;

        if (categoria.RangoPesoMin.HasValue && competidor.Peso < categoria.RangoPesoMin.Value)
            return false;
        if (categoria.RangoPesoMax.HasValue && competidor.Peso > categoria.RangoPesoMax.Value)
            return false;

        if (Enum.TryParse<Graduacion>(competidor.Graduacion, out var g)
            && Enum.TryParse<Graduacion>(categoria.RangoGraduacionMin, out var gMin)
            && Enum.TryParse<Graduacion>(categoria.RangoGraduacionMax, out var gMax)
            && (g < gMin || g > gMax))
        {
            return false;
        }

        return true;
    }
}
