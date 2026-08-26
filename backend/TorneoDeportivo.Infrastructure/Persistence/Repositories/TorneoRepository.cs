using Microsoft.EntityFrameworkCore;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación de <see cref="ITorneoRepository"/> basada en EF Core sobre <see cref="TorneoDbContext"/>.
/// </summary>
public class TorneoRepository(TorneoDbContext db) : ITorneoRepository
{
    /// <summary>Busca un torneo por su id; devuelve null si no existe.</summary>
    public Task<Torneo?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Torneos.FirstOrDefaultAsync(t => t.Id == id, ct);

    /// <summary>Obtiene todos los torneos que no están en estado Finalizado.</summary>
    public Task<List<Torneo>> GetAllAsync(CancellationToken ct) =>
        db.Torneos.OrderByDescending(t => t.Fecha).ToListAsync(ct);

    /// <summary>Agrega un nuevo torneo y guarda los cambios.</summary>
    public async Task AddAsync(Torneo torneo, CancellationToken ct)
    {
        db.Torneos.Add(torneo);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Actualiza un torneo existente y guarda los cambios.</summary>
    public async Task UpdateAsync(Torneo torneo, CancellationToken ct)
    {
        db.Torneos.Update(torneo);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Elimina un torneo y guarda los cambios.</summary>
    public async Task DeleteAsync(Torneo torneo, CancellationToken ct)
    {
        db.Torneos.Remove(torneo);
        await db.SaveChangesAsync(ct);
    }
}
