using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Categorias.Commands.DeleteCategoria;

/// <summary>
/// Handler CQRS que elimina una categoría: valida que exista y pertenezca al torneo indicado e impide eliminar
/// categorías de un torneo finalizado. Los competidores asignados quedan sin categoría (FK ON DELETE SET NULL).
/// </summary>
public class DeleteCategoriaCommandHandler(
    ICategoriaRepository categoriaRepo,
    ITorneoRepository torneoRepo) : IRequestHandler<DeleteCategoriaCommand>
{
    /// <summary>
    /// Busca la categoría (404 si no existe o no pertenece al torneo de la ruta), rechaza la eliminación si el
    /// torneo está Finalizado (409) y borra el registro.
    /// </summary>
    public async Task Handle(DeleteCategoriaCommand request, CancellationToken ct)
    {
        var categoria = await categoriaRepo.GetByIdAsync(request.Id, ct);
        if (categoria is null || categoria.TorneoId != request.TorneoId)
            throw new NotFoundException(nameof(Categoria), request.Id);

        var torneo = await torneoRepo.GetByIdAsync(categoria.TorneoId, ct);
        if (torneo?.Estado == EstadoTorneo.Finalizado)
            throw new ConflictException("No se pueden eliminar las categorías de un torneo finalizado.");

        await categoriaRepo.DeleteAsync(categoria, ct);
    }
}
