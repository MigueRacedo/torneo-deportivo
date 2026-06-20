using Microsoft.EntityFrameworkCore;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Persistence.Repositories;

public class TorneoRepository(TorneoDbContext db) : ITorneoRepository
{
    public Task<Torneo?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Torneos.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<List<Torneo>> GetAllActivosAsync(CancellationToken ct) =>
        db.Torneos.Where(t => t.Estado != EstadoTorneo.Finalizado).ToListAsync(ct);

    public async Task AddAsync(Torneo torneo, CancellationToken ct)
    {
        db.Torneos.Add(torneo);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Torneo torneo, CancellationToken ct)
    {
        db.Torneos.Update(torneo);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Torneo torneo, CancellationToken ct)
    {
        db.Torneos.Remove(torneo);
        await db.SaveChangesAsync(ct);
    }
}
