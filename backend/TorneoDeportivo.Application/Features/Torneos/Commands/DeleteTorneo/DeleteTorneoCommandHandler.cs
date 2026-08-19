using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.DeleteTorneo;

/// <summary>
/// Handler CQRS que elimina un torneo: solo permite hacerlo si está en estado Borrador. Al borrarlo, sus
/// categorías, competidores y llaves se eliminan en cascada.
/// </summary>
public class DeleteTorneoCommandHandler(ITorneoRepository repo) : IRequestHandler<DeleteTorneoCommand>
{
    /// <summary>
    /// Busca el torneo (404 si no existe), rechaza la eliminación si no está en Borrador (409) y borra el registro.
    /// </summary>
    public async Task Handle(DeleteTorneoCommand request, CancellationToken ct)
    {
        var torneo = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Torneo), request.Id);

        if (torneo.Estado != EstadoTorneo.Borrador)
            throw new ConflictException("Solo se pueden eliminar torneos en estado Borrador.");

        await repo.DeleteAsync(torneo, ct);
    }
}
