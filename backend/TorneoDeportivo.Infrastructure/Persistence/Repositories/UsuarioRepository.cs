using Microsoft.EntityFrameworkCore;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Infrastructure.Persistence.Repositories;

public class UsuarioRepository(TorneoDbContext db) : IUsuarioRepository
{
    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct) =>
        db.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> ExisteAlgunoAsync(CancellationToken ct) =>
        db.Usuarios.AnyAsync(ct);

    public async Task AddAsync(Usuario usuario, CancellationToken ct)
    {
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(ct);
    }
}
