namespace TorneoDeportivo.Domain.Enums;

/// <summary>
/// Estado de un enfrentamiento (<see cref="Entities.LlaveCompetencia"/>) dentro del bracket.
/// </summary>
public enum EstadoLlave
{
    Pendiente,
    EnCurso,
    Finalizado,
    Bye
}
