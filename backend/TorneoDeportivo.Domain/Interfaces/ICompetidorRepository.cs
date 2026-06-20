using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

public interface ICompetidorRepository
{
    Task<Competidor?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Competidor>> GetByTorneoIdAsync(Guid torneoId, CancellationToken ct);
    Task<List<Competidor>> GetByEscuelaAsync(Guid torneoId, string escuela, CancellationToken ct);
    Task AddAsync(Competidor competidor, CancellationToken ct);
}
