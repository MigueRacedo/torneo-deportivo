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

        // Fail-closed: si alguna de las tres graduaciones no parsea, el competidor NO encaja.
        // Antes las tres condiciones iban encadenadas con && junto a la comparación de rango, así que un
        // parseo fallido cortocircuitaba toda la expresión y el chequeo de graduación se salteaba en
        // silencio, dando por bueno el encaje. Rechazar deja al competidor visible en "sin categoría",
        // que es donde se puede detectar y corregir el dato.
        if (!Enum.TryParse<Graduacion>(competidor.Graduacion, out var g)
            || !Enum.TryParse<Graduacion>(categoria.RangoGraduacionMin, out var gMin)
            || !Enum.TryParse<Graduacion>(categoria.RangoGraduacionMax, out var gMax))
        {
            return false;
        }

        return g >= gMin && g <= gMax;
    }
}
