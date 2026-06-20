using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

public interface ITorneoRepository
{
    Task<Torneo?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Torneo>> GetAllActivosAsync(CancellationToken ct);
    Task AddAsync(Torneo torneo, CancellationToken ct);
    Task UpdateAsync(Torneo torneo, CancellationToken ct);
    Task DeleteAsync(Torneo torneo, CancellationToken ct);
}
