using Microsoft.EntityFrameworkCore;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación de <see cref="ICategoriaRepository"/> basada en EF Core sobre <see cref="TorneoDbContext"/>.
/// </summary>
public class CategoriaRepository(TorneoDbContext db) : ICategoriaRepository
{
    /// <summary>Busca una categoría por su id; devuelve null si no existe.</summary>
    public Task<Categoria?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Categorias.FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <summary>Obtiene todas las categorías de un torneo ordenadas por nombre.</summary>
    public Task<List<Categoria>> GetByTorneoIdAsync(Guid torneoId, CancellationToken ct) =>
        db.Categorias
            .Where(c => c.TorneoId == torneoId)
            .OrderBy(c => c.Nombre)
            .ToListAsync(ct);

    /// <summary>Agrega una nueva categoría y guarda los cambios.</summary>
    public async Task AddAsync(Categoria categoria, CancellationToken ct)
    {
        db.Categorias.Add(categoria);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Actualiza una categoría existente y guarda los cambios.</summary>
    public async Task UpdateAsync(Categoria categoria, CancellationToken ct)
    {
        db.Categorias.Update(categoria);
        await db.SaveChangesAsync(ct);
    }
}
