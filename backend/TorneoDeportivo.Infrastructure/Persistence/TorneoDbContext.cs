using Microsoft.EntityFrameworkCore;
using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Infrastructure.Persistence;

/// <summary>
/// Contexto de EF Core del sistema: expone los DbSet de todas las entidades del dominio y aplica
/// las configuraciones Fluent API definidas en el ensamblado de Infrastructure.
/// </summary>
public class TorneoDbContext(DbContextOptions<TorneoDbContext> options) : DbContext(options)
{
    public DbSet<Torneo> Torneos => Set<Torneo>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Competidor> Competidores => Set<Competidor>();
    public DbSet<LlaveCompetencia> Llaves => Set<LlaveCompetencia>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>Aplica automáticamente todas las clases de configuración (IEntityTypeConfiguration) del ensamblado.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TorneoDbContext).Assembly);
    }
}
