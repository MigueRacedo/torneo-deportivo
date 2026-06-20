using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Infrastructure.Persistence.Configurations;

public class CompetidorConfiguration : IEntityTypeConfiguration<Competidor>
{
    public void Configure(EntityTypeBuilder<Competidor> builder)
    {
        builder.ToTable("competidores");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Apellido).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Graduacion).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Escuela).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Responsable).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Telefono).HasMaxLength(50);
    }
}
