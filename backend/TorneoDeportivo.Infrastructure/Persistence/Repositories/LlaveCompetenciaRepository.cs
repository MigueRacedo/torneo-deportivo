using Microsoft.EntityFrameworkCore;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación de <see cref="ILlaveCompetenciaRepository"/> basada en EF Core sobre <see cref="TorneoDbContext"/>.
/// </summary>
public class LlaveCompetenciaRepository(TorneoDbContext db) : ILlaveCompetenciaRepository
{
    /// <summary>Obtiene las llaves de una categoría (con sus competidores) ordenadas por ronda y posición.</summary>
    public Task<List<LlaveCompetencia>> GetByCategoriaIdAsync(Guid categoriaId, CancellationToken ct) =>
        db.Llaves
            .Include(l => l.Competidor1)
            .Include(l => l.Competidor2)
            .Include(l => l.Ganador)
            .Where(l => l.CategoriaId == categoriaId)
            .OrderBy(l => l.Ronda).ThenBy(l => l.Posicion)
            .ToListAsync(ct);

    /// <summary>Busca una llave por su id (con sus competidores); devuelve null si no existe.</summary>
    public Task<LlaveCompetencia?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Llaves
            .Include(l => l.Competidor1)
            .Include(l => l.Competidor2)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    /// <summary>Persiste en bloque el conjunto de llaves generado para un bracket.</summary>
    public async Task AddRangeAsync(IEnumerable<LlaveCompetencia> llaves, CancellationToken ct)
    {
        db.Llaves.AddRange(llaves);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Actualiza una llave existente y guarda los cambios.</summary>
    public async Task UpdateAsync(LlaveCompetencia llave, CancellationToken ct)
    {
        db.Llaves.Update(llave);
        await db.SaveChangesAsync(ct);
    }
}
