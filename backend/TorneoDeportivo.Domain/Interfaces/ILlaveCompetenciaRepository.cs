using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

public interface ILlaveCompetenciaRepository
{
    Task<List<LlaveCompetencia>> GetByCategoriaIdAsync(Guid categoriaId, CancellationToken ct);
    Task<LlaveCompetencia?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddRangeAsync(IEnumerable<LlaveCompetencia> llaves, CancellationToken ct);
    Task UpdateAsync(LlaveCompetencia llave, CancellationToken ct);
}
