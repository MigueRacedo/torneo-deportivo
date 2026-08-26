using Microsoft.EntityFrameworkCore;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación de <see cref="ICompetidorRepository"/> basada en EF Core sobre <see cref="TorneoDbContext"/>.
/// </summary>
public class CompetidorRepository(TorneoDbContext db) : ICompetidorRepository
{
    /// <summary>Busca un competidor por su id; devuelve null si no existe.</summary>
    public Task<Competidor?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Competidores.FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <summary>Obtiene los competidores de un torneo (con su categoría) ordenados por apellido y nombre.</summary>
    public Task<List<Competidor>> GetByTorneoIdAsync(Guid torneoId, CancellationToken ct) =>
        db.Competidores
            .Include(c => c.Categoria)
            .Where(c => c.TorneoId == torneoId)
            .OrderBy(c => c.Apellido).ThenBy(c => c.Nombre)
            .ToListAsync(ct);

    /// <summary>Obtiene los competidores de un torneo de una escuela específica (vista del Profesor).</summary>
    public Task<List<Competidor>> GetByEscuelaAsync(Guid torneoId, string escuela, CancellationToken ct) =>
        db.Competidores
            .Include(c => c.Categoria)
            .Where(c => c.TorneoId == torneoId && c.Escuela == escuela)
            .OrderBy(c => c.Apellido).ThenBy(c => c.Nombre)
            .ToListAsync(ct);

    /// <summary>
    /// Cuenta los competidores del torneo agrupados por categoría. La agregación se resuelve en SQL
    /// (GROUP BY) en lugar de traer los competidores y contarlos en memoria.
    /// </summary>
    public async Task<Dictionary<Guid, int>> GetConteoPorCategoriaAsync(Guid torneoId, CancellationToken ct) =>
        await db.Competidores
            .Where(c => c.TorneoId == torneoId && c.CategoriaId != null)
            .GroupBy(c => c.CategoriaId!.Value)
            .Select(g => new { CategoriaId = g.Key, Total = g.Count() })
            .ToDictionaryAsync(x => x.CategoriaId, x => x.Total, ct);

    /// <summary>Agrega un nuevo competidor y guarda los cambios.</summary>
    public async Task AddAsync(Competidor competidor, CancellationToken ct)
    {
        db.Competidores.Add(competidor);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Actualiza un competidor existente y guarda los cambios.</summary>
    public async Task UpdateAsync(Competidor competidor, CancellationToken ct)
    {
        db.Competidores.Update(competidor);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Elimina un competidor y guarda los cambios.</summary>
    public async Task DeleteAsync(Competidor competidor, CancellationToken ct)
    {
        db.Competidores.Remove(competidor);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Actualiza en bloque un conjunto de competidores y guarda los cambios.</summary>
    public async Task UpdateRangeAsync(IEnumerable<Competidor> competidores, CancellationToken ct)
    {
        db.Competidores.UpdateRange(competidores);
        await db.SaveChangesAsync(ct);
    }
}
