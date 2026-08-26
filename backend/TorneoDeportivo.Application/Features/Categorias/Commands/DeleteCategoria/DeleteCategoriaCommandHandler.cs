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

        if (categoria.LlavesGeneradas)
            throw new ConflictException("No se puede eliminar una categoría con llaves generadas.");

        var torneo = await torneoRepo.GetByIdAsync(categoria.TorneoId, ct);
        if (torneo is null || torneo.Estado != EstadoTorneo.Borrador)
            throw new ConflictException("No se pueden eliminar las categorías de un torneo que ya arrancó. Solo se puede en Borrador.");

        await categoriaRepo.DeleteAsync(categoria, ct);
    }
}
