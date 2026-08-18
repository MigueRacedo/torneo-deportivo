using Mapster;
using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.UpdateTorneo;

/// <summary>
/// Handler CQRS que edita un torneo existente: valida que exista, impide modificar torneos finalizados
/// (solo lectura) y persiste los nuevos datos.
/// </summary>
public class UpdateTorneoCommandHandler(ITorneoRepository repo) : IRequestHandler<UpdateTorneoCommand, TorneoResponse>
{
    /// <summary>
    /// Busca el torneo (lanza <see cref="NotFoundException"/> si no existe), rechaza la edición si está
    /// Finalizado (<see cref="ConflictException"/>), aplica los cambios y devuelve la representación actualizada.
    /// </summary>
    public async Task<TorneoResponse> Handle(UpdateTorneoCommand request, CancellationToken ct)
    {
        var torneo = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Torneo), request.Id);

        if (torneo.Estado == EstadoTorneo.Finalizado)
            throw new ConflictException("No se puede editar un torneo finalizado.");

        torneo.Nombre = request.Nombre;
        torneo.Fecha = request.Fecha;
        torneo.Lugar = request.Lugar;
        torneo.ImagenFlyer = request.ImagenFlyer;

        await repo.UpdateAsync(torneo, ct);

        return torneo.Adapt<TorneoResponse>();
    }
}
