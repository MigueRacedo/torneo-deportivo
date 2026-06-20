using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Domain.Entities;

public class LlaveCompetencia
{
    public Guid Id { get; set; }
    public Guid CategoriaId { get; set; }
    public int Ronda { get; set; }
    public int Posicion { get; set; }
    public Guid? Competidor1Id { get; set; }
    public Guid? Competidor2Id { get; set; }
    public Guid? GanadorId { get; set; }
    public EstadoLlave Estado { get; set; } = EstadoLlave.Pendiente;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Categoria Categoria { get; set; } = null!;
    public Competidor? Competidor1 { get; set; }
    public Competidor? Competidor2 { get; set; }
    public Competidor? Ganador { get; set; }
}
