namespace TorneoDeportivo.Domain.Enums;

/// <summary>
/// Rol de un <see cref="Entities.Usuario"/>, usado para autorización basada en roles en la API.
/// </summary>
public enum RolUsuario
{
    Coordinador,
    Profesor,
    Director
}
