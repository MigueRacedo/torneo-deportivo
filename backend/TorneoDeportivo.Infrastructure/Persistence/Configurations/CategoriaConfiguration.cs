using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de EF Core para el mapeo de <see cref="Categoria"/> a la tabla "categorias".
/// </summary>
public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    /// <summary>Define columnas, tipos, longitudes y relaciones de la entidad Categoria.</summary>
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("categorias");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(c => c.TipoCompetencia).HasConversion<string>();
        builder.Property(c => c.Sexo).HasConversion<string>();
        builder.Property(c => c.RangoPesoMin).HasPrecision(5, 2);
        builder.Property(c => c.RangoPesoMax).HasPrecision(5, 2);
        builder.Property(c => c.RangoGraduacionMin).IsRequired().HasMaxLength(50);
        builder.Property(c => c.RangoGraduacionMax).IsRequired().HasMaxLength(50);
        builder.HasMany(c => c.Competidores).WithOne(comp => comp.Categoria)
               .HasForeignKey(comp => comp.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(c => c.Llaves).WithOne(l => l.Categoria)
               .HasForeignKey(l => l.CategoriaId).OnDelete(DeleteBehavior.Cascade);
    }
}
