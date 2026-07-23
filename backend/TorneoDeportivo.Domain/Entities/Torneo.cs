using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Domain.Entities;

/// <summary>
/// Evento deportivo raíz del sistema: agrupa las categorías y competidores inscriptos, con su estado
/// de ciclo de vida (Borrador, Activo, Finalizado).
/// </summary>
public class Torneo
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public string? ImagenFlyer { get; set; }
    public EstadoTorneo Estado { get; set; } = EstadoTorneo.Borrador;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Categoria> Categorias { get; set; } = [];
    public ICollection<Competidor> Competidores { get; set; } = [];
}
