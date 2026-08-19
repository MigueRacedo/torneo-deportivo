using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Competidores.Commands.DeleteCompetidor;

/// <summary>
/// Handler CQRS que elimina un competidor: valida que exista y pertenezca al torneo indicado e impide eliminar
/// competidores de un torneo finalizado.
/// </summary>
public class DeleteCompetidorCommandHandler(
    ICompetidorRepository competidorRepo,
    ITorneoRepository torneoRepo) : IRequestHandler<DeleteCompetidorCommand>
{
    /// <summary>
    /// Busca el competidor (404 si no existe o no pertenece al torneo de la ruta), rechaza la eliminación si el
    /// torneo está Finalizado (409) y borra el registro.
    /// </summary>
    public async Task Handle(DeleteCompetidorCommand request, CancellationToken ct)
    {
        var competidor = await competidorRepo.GetByIdAsync(request.Id, ct);
        if (competidor is null || competidor.TorneoId != request.TorneoId)
            throw new NotFoundException(nameof(Competidor), request.Id);

        var torneo = await torneoRepo.GetByIdAsync(competidor.TorneoId, ct);
        if (torneo?.Estado == EstadoTorneo.Finalizado)
            throw new ConflictException("No se pueden eliminar competidores de un torneo finalizado.");

        await competidorRepo.DeleteAsync(competidor, ct);
    }
}
