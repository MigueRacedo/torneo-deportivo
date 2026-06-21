using TorneoDeportivo.Domain.Entities;

namespace TorneoDeportivo.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Usuario usuario);
}
