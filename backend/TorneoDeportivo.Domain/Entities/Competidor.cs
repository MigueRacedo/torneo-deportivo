namespace TorneoDeportivo.Domain.Entities;

public class Competidor
{
    public Guid Id { get; set; }
    public Guid TorneoId { get; set; }
    public Guid CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public int Edad { get; set; }
    public string Graduacion { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public decimal Altura { get; set; }
    public string Escuela { get; set; } = string.Empty;
    public string Responsable { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Torneo Torneo { get; set; } = null!;
    public Categoria Categoria { get; set; } = null!;
}
