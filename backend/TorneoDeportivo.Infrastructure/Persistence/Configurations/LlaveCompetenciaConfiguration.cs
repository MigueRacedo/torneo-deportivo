using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Infrastructure.Persistence.Configurations;

public class LlaveCompetenciaConfiguration : IEntityTypeConfiguration<LlaveCompetencia>
{
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
