using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de EF Core para el mapeo de <see cref="LlaveCompetencia"/> a la tabla
/// "llaves_competencia", incluyendo las relaciones con los competidores del enfrentamiento y el ganador.
/// </summary>
public class LlaveCompetenciaConfiguration : IEntityTypeConfiguration<LlaveCompetencia>
{
    /// <summary>Define columnas, conversión de enum y relaciones de la entidad LlaveCompetencia.</summary>
    public void Configure(EntityTypeBuilder<LlaveCompetencia> builder)
    {
        builder.ToTable("llaves_competencia");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(l => l.Estado).HasConversion<string>();

        builder.HasOne(l => l.Competidor1).WithMany()
               .HasForeignKey(l => l.Competidor1Id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.Competidor2).WithMany()
               .HasForeignKey(l => l.Competidor2Id).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.Ganador).WithMany()
               .HasForeignKey(l => l.GanadorId).OnDelete(DeleteBehavior.Restrict);
    }
}
