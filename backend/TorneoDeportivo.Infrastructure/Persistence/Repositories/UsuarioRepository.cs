using Microsoft.EntityFrameworkCore;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación de <see cref="IUsuarioRepository"/> basada en EF Core sobre <see cref="TorneoDbContext"/>.
/// </summary>
public class UsuarioRepository(TorneoDbContext db) : IUsuarioRepository
{
    /// <summary>Busca un usuario por su id; devuelve null si no existe.</summary>
    public Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    /// <summary>Busca un usuario por su email; devuelve null si no existe.</summary>
    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct) =>
        db.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

    /// <summary>Agrega un nuevo usuario y guarda los cambios.</summary>
    public async Task AddAsync(Usuario usuario, CancellationToken ct)
    {
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(ct);
    }
}
