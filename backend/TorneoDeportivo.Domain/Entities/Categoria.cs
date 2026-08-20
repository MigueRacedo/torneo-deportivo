using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Domain.Entities;

/// <summary>
/// Categoría de competencia dentro de un torneo (e.g. combate femenino, 12-13 años, cinturón amarillo-verde),
/// que agrupa a los competidores elegibles y sus llaves de eliminación.
/// </summary>
public class Categoria
{
    public Guid Id { get; set; }
    public Guid TorneoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoCompetencia TipoCompetencia { get; set; }
    public Sexo Sexo { get; set; }
    // Rangos abiertos: null en el mínimo = "hasta X"; null en el máximo = "X en adelante".
    public int? RangoEdadMin { get; set; }
    public int? RangoEdadMax { get; set; }
    // Peso solo aplica a Combate; en Formas queda null.
    public decimal? RangoPesoMin { get; set; }
    public decimal? RangoPesoMax { get; set; }
    public string RangoGraduacionMin { get; set; } = string.Empty;
    public string RangoGraduacionMax { get; set; } = string.Empty;
    // True una vez que se generaron las llaves de esta categoría (impide regenerarlas).
    public bool LlavesGeneradas { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Torneo Torneo { get; set; } = null!;
    public ICollection<Competidor> Competidores { get; set; } = [];
    public ICollection<LlaveCompetencia> Llaves { get; set; } = [];
}
