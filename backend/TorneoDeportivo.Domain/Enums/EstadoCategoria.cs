namespace TorneoDeportivo.Domain.Enums;

/// <summary>
/// Estado del ciclo de vida de una <see cref="Entities.Categoria"/> dentro de un torneo (H0009).
/// </summary>
/// <remarks>
/// El orden de los valores es significativo: se comparan con &gt;= para saber si una categoría ya pasó
/// por una etapa (por ejemplo, "¿ya tiene llaves?" es <c>Estado &gt;= LlavesGeneradas</c>). No reordenar.
/// </remarks>
public enum EstadoCategoria
{
    /// <summary>Todavía no se generó el bracket. La categoría se puede editar y eliminar.</summary>
    SinLlaves = 0,

    /// <summary>Ya tiene bracket armado, pero no se registró ningún resultado.</summary>
    LlavesGeneradas = 1,

    /// <summary>Se registró al menos un ganador: la categoría está compitiendo.</summary>
    EnCurso = 2,

    /// <summary>Todos sus enfrentamientos están resueltos: hay campeón.</summary>
    Finalizada = 3
}
