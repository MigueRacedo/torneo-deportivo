using Mapster;
using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Categorias.Commands.UpdateCategoria;

/// <summary>
/// Handler CQRS que edita una categoría existente: valida que exista y pertenezca al torneo indicado,
/// impide editar categorías de un torneo finalizado, convierte los enums, descarta el peso si es de Formas
/// y persiste los cambios.
/// </summary>
public class UpdateCategoriaCommandHandler(ICategoriaRepository repo, ITorneoRepository torneoRepo)
    : IRequestHandler<UpdateCategoriaCommand, CategoriaResponse>
{
    /// <summary>
    /// Busca la categoría (lanza <see cref="NotFoundException"/> si no existe o no pertenece al torneo de la ruta),
    /// rechaza la edición si el torneo está Finalizado (<see cref="ConflictException"/>), aplica los nuevos datos
    /// y devuelve la representación actualizada.
    /// </summary>
    public async Task<CategoriaResponse> Handle(UpdateCategoriaCommand request, CancellationToken ct)
    {
        var categoria = await repo.GetByIdAsync(request.Id, ct);
        if (categoria is null || categoria.TorneoId != request.TorneoId)
            throw new NotFoundException(nameof(Categoria), request.Id);

        if (categoria.LlavesGeneradas)
            throw new ConflictException("No se puede editar una categoría con llaves generadas.");

        var torneo = await torneoRepo.GetByIdAsync(categoria.TorneoId, ct);
        if (torneo is null || torneo.Estado != EstadoTorneo.Borrador)
            throw new ConflictException("No se pueden editar las categorías de un torneo que ya arrancó. Solo se puede en Borrador.");

        var tipoCompetencia = Enum.Parse<TipoCompetencia>(request.TipoCompetencia);
        // El peso solo aplica a Combate; en Formas se descarta cualquier valor recibido.
        var esFormas = tipoCompetencia == TipoCompetencia.Formas;

        categoria.Nombre = request.Nombre;
        categoria.TipoCompetencia = tipoCompetencia;
        categoria.Sexo = Enum.Parse<Sexo>(request.Sexo);
        categoria.RangoEdadMin = request.RangoEdadMin;
        categoria.RangoEdadMax = request.RangoEdadMax;
        categoria.RangoPesoMin = esFormas ? null : request.RangoPesoMin;
        categoria.RangoPesoMax = esFormas ? null : request.RangoPesoMax;
        categoria.RangoGraduacionMin = request.RangoGraduacionMin;
        categoria.RangoGraduacionMax = request.RangoGraduacionMax;

        await repo.UpdateAsync(categoria, ct);

        return categoria.Adapt<CategoriaResponse>();
    }
}
