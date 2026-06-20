using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Infrastructure.Persistence.Configurations;

public class TorneoConfiguration : IEntityTypeConfiguration<Torneo>
{
    public void Configure(EntityTypeBuilder<Torneo> builder)
    {
        builder.ToTable("torneos");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Lugar).IsRequired().HasMaxLength(300);
        builder.Property(t => t.Estado).HasConversion<string>();
        builder.HasMany(t => t.Categorias).WithOne(c => c.Torneo)
               .HasForeignKey(c => c.TorneoId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(t => t.Competidores).WithOne(c => c.Torneo)
               .HasForeignKey(c => c.TorneoId).OnDelete(DeleteBehavior.Cascade);
    }
}
