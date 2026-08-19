using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Domain.Entities;

/// <summary>
/// Deportista inscripto en un torneo, con sus datos personales, físicos y de escuela. La categoría es opcional:
/// se carga sin categoría y se asigna al armar las llaves (H0005), clasificándolo por sus atributos.
/// </summary>
public class Competidor
{
    public Guid Id { get; set; }
    public Guid TorneoId { get; set; }
    public Guid? CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public Sexo Sexo { get; set; }
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
    public Categoria? Categoria { get; set; }
}
