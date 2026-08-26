using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Competidores.Queries.GetMisCompetidores;

/// <summary>
/// Handler CQRS que devuelve los competidores del torneo que pertenecen a la escuela del Profesor
/// autenticado, validando antes que el torneo y el usuario existan.
/// </summary>
public class GetMisCompetidoresQueryHandler(
    ITorneoRepository torneoRepo,
    IUsuarioRepository usuarioRepo,
    ICompetidorRepository competidorRepo) : IRequestHandler<GetMisCompetidoresQuery, MisCompetidoresResponse>
{
    /// <summary>
    /// Verifica el torneo (404), lee la escuela del usuario autenticado y devuelve sus alumnos inscriptos.
    /// Si el usuario no tiene escuela asignada devuelve la respuesta vacía, no un error.
    /// </summary>
    public async Task<MisCompetidoresResponse> Handle(GetMisCompetidoresQuery request, CancellationToken ct)
    {
        var torneo = await torneoRepo.GetByIdAsync(request.TorneoId, ct)
            ?? throw new NotFoundException(nameof(Torneo), request.TorneoId);

        var usuario = await usuarioRepo.GetByIdAsync(request.UsuarioId, ct)
            ?? throw new NotFoundException(nameof(Usuario), request.UsuarioId);

        // Un Profesor sin escuela cargada no es un error del sistema: es un perfil incompleto que el
        // Coordinador tiene que completar. Se devuelve vacío y el cliente muestra el mensaje accionable.
        if (string.IsNullOrWhiteSpace(usuario.Escuela))
            return new MisCompetidoresResponse(null, []);

        var competidores = await competidorRepo.GetByEscuelaAsync(torneo.Id, usuario.Escuela, ct);

        return new MisCompetidoresResponse(usuario.Escuela, competidores.Select(c => c.ToResponse()).ToList());
    }
}
