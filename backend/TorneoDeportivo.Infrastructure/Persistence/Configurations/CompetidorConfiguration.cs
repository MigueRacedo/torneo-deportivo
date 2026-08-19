using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de EF Core para el mapeo de <see cref="Competidor"/> a la tabla "competidores".
/// </summary>
public class CompetidorConfiguration : IEntityTypeConfiguration<Competidor>
{
    /// <summary>Define columnas, tipos y longitudes de la entidad Competidor.</summary>
    public void Configure(EntityTypeBuilder<Competidor> builder)
    {
        builder.ToTable("competidores");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Apellido).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Sexo).HasConversion<string>();
        builder.Property(c => c.Peso).HasPrecision(5, 2);
        builder.Property(c => c.Altura).HasPrecision(4, 2);
        builder.Property(c => c.Graduacion).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Escuela).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Responsable).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Telefono).HasMaxLength(50);
    }
}
