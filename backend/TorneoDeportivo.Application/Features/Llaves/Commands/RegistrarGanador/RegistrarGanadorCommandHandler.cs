using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Common.Interfaces;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Llaves.Commands.RegistrarGanador;

/// <summary>
/// Handler CQRS que registra el ganador de un match: valida que el match exista y pertenezca al torneo y que el
/// ganador sea uno de sus dos competidores, marca el match como Finalizado, avanza al ganador a la ronda siguiente
/// y notifica el cambio en vivo (SignalR).
/// </summary>
public class RegistrarGanadorCommandHandler(
    ILlaveCompetenciaRepository llaveRepo,
    ICategoriaRepository categoriaRepo,
    IBracketNotifier notifier) : IRequestHandler<RegistrarGanadorCommand, MatchResponse>
{
    /// <summary>
    /// Verifica el match y el torneo (404), valida el ganador (409), lo registra, lo propaga al match siguiente
    /// y emite la notificación en vivo.
    /// </summary>
    public async Task<MatchResponse> Handle(RegistrarGanadorCommand request, CancellationToken ct)
    {
        var llave = await llaveRepo.GetByIdAsync(request.LlaveId, ct)
            ?? throw new NotFoundException(nameof(LlaveCompetencia), request.LlaveId);

        var categoria = await categoriaRepo.GetByIdAsync(llave.CategoriaId, ct);
        if (categoria is null || categoria.TorneoId != request.TorneoId)
            throw new NotFoundException(nameof(LlaveCompetencia), request.LlaveId);

        // Un match ya resuelto no se reabre: si se cambiara el ganador después de que la ronda siguiente
        // avanzó, el bracket quedaría inconsistente (el perdedor seguiría propagado aguas abajo).
        if (llave.Estado == EstadoLlave.Finalizado)
            throw new ConflictException("El ganador de este match ya fue registrado.");

        if (llave.Estado == EstadoLlave.Bye)
            throw new ConflictException("Un match con bye no lleva ganador: el competidor ya avanzó de ronda.");

        // Guard de integridad del bracket: sin los dos competidores definidos no hay enfrentamiento que
        // resolver. Sin esto se podría avanzar a alguien cuyo rival todavía no salió de la ronda anterior.
        if (llave.Competidor1Id is null || llave.Competidor2Id is null)
            throw new ConflictException("El match todavía no tiene definidos sus dos competidores.");

        if (request.GanadorId != llave.Competidor1Id && request.GanadorId != llave.Competidor2Id)
            throw new ConflictException("El ganador debe ser uno de los competidores del match.");

        // Trabajar sobre el conjunto completo de la categoría para poder propagar al match siguiente.
        var todas = await llaveRepo.GetByCategoriaIdAsync(llave.CategoriaId, ct);
        var actual = todas.First(l => l.Id == llave.Id);

        actual.GanadorId = request.GanadorId;
        actual.Estado = EstadoLlave.Finalizado;
        actual.Ganador = actual.Competidor1Id == request.GanadorId ? actual.Competidor1 : actual.Competidor2;

        // El match actual y el siguiente se guardan juntos: si se persistieran por separado y fallara el
        // segundo, quedaría un competidor avanzado sin que su match figure como finalizado (o viceversa).
        var modificadas = new List<LlaveCompetencia> { actual };

        var siguiente = todas.FirstOrDefault(l => l.Ronda == actual.Ronda + 1 && l.Posicion == actual.Posicion / 2);
        if (siguiente is not null)
        {
            if (actual.Posicion % 2 == 0) siguiente.Competidor1Id = request.GanadorId;
            else siguiente.Competidor2Id = request.GanadorId;
            modificadas.Add(siguiente);
        }

        await llaveRepo.UpdateRangeAsync(modificadas, ct);
        await notifier.NotificarMatchActualizadoAsync(request.TorneoId, categoria.Id, ct);

        return actual.ToMatchResponse();
    }
}
