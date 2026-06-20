using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

public interface ICategoriaRepository
{
    Task<Categoria?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Categoria>> GetByTorneoIdAsync(Guid torneoId, CancellationToken ct);
    Task AddAsync(Categoria categoria, CancellationToken ct);
    Task UpdateAsync(Categoria categoria, CancellationToken ct);
}
