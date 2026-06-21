using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct);
    Task<bool> ExisteAlgunoAsync(CancellationToken ct);
    Task AddAsync(Usuario usuario, CancellationToken ct);
}
