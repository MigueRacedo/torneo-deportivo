using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Domain.Entities;

public class Categoria
{
    public Guid Id { get; set; }
    public Guid TorneoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoCompetencia TipoCompetencia { get; set; }
    public Sexo Sexo { get; set; }
    public int RangoEdadMin { get; set; }
    public int RangoEdadMax { get; set; }
    public decimal RangoPesoMin { get; set; }
    public decimal RangoPesoMax { get; set; }
    public string RangoGraduacionMin { get; set; } = string.Empty;
    public string RangoGraduacionMax { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Torneo Torneo { get; set; } = null!;
    public ICollection<Competidor> Competidores { get; set; } = [];
    public ICollection<LlaveCompetencia> Llaves { get; set; } = [];
}
