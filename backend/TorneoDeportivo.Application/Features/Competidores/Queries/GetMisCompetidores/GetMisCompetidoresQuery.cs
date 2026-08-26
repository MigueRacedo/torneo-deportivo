using MediatR;

namespace TorneoDeportivo.Application.Features.Competidores.Queries.GetMisCompetidores;

/// <summary>
/// Query CQRS que pide los competidores de un torneo pertenecientes a la escuela del Profesor autenticado (H0007).
/// </summary>
/// <remarks>
/// Recibe el <paramref name="UsuarioId"/> y NO la escuela: si la escuela viajara como parámetro, cualquier
/// Profesor podría pedir la de otro y leer datos de contacto de alumnos ajenos. La escuela se resuelve
/// server-side a partir del usuario autenticado.
/// </remarks>
public record GetMisCompetidoresQuery(Guid TorneoId, Guid UsuarioId) : IRequest<MisCompetidoresResponse>;

/// <summary>
/// Alumnos de una escuela en un torneo. <see cref="Escuela"/> viene en null cuando el usuario todavía
/// no tiene escuela asignada, para que el cliente lo distinga de "la escuela no tiene inscriptos".
/// </summary>
public record MisCompetidoresResponse(string? Escuela, List<CompetidorResponse> Competidores);
