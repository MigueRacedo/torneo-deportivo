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
/// Alumno tal como lo ve su Profesor: solo los datos deportivos que muestra la consulta.
/// </summary>
/// <remarks>
/// Es un DTO propio y no <c>CompetidorResponse</c> a propósito: aquel incluye <c>Responsable</c> y
/// <c>Telefono</c>, que esta vista no muestra. Mandarlos igual dejaría datos de contacto de menores
/// viajando en una respuesta que nadie usa, y bastaría con abrir las devtools para verlos.
/// </remarks>
public record MiAlumnoResponse(
    Guid Id,
    string NombreCompleto,
    string Sexo,
    int Edad,
    string Graduacion,
    decimal Peso,
    string? CategoriaNombre);

/// <summary>
/// Alumnos de una escuela en un torneo. <see cref="Escuela"/> viene en null cuando el usuario todavía
/// no tiene escuela asignada, para que el cliente lo distinga de "la escuela no tiene inscriptos".
/// </summary>
public record MisCompetidoresResponse(string? Escuela, List<MiAlumnoResponse> Competidores);
