using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración Fluent API de EF Core para el mapeo de <see cref="Usuario"/> a la tabla "usuarios".
/// </summary>
public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    /// <summary>Define columnas, tipos, longitudes y el índice único de email de la entidad Usuario.</summary>
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(u => u.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Rol).HasConversion<string>();
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
