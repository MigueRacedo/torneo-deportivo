using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Domain.Entities;

/// <summary>
/// Usuario del sistema con credenciales de acceso y un rol (Coordinador, Profesor o Director)
/// que determina sus permisos.
/// </summary>
public class Usuario
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
