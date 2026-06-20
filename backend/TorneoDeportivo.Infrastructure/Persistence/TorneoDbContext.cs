using Microsoft.EntityFrameworkCore;
using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Infrastructure.Persistence;

public class TorneoDbContext(DbContextOptions<TorneoDbContext> options) : DbContext(options)
{
    public DbSet<Torneo> Torneos => Set<Torneo>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Competidor> Competidores => Set<Competidor>();
    public DbSet<LlaveCompetencia> Llaves => Set<LlaveCompetencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TorneoDbContext).Assembly);
    }
}
