using Mapster;
using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Categorias.Commands.CreateCategoria;

/// <summary>
/// Handler CQRS que crea una categoría dentro de un torneo existente: verifica que el torneo exista,
/// convierte los enums de texto y persiste la categoría mediante el repositorio.
/// </summary>
public class CreateCategoriaCommandHandler(
    ICategoriaRepository categoriaRepo,
    ITorneoRepository torneoRepo) : IRequestHandler<CreateCategoriaCommand, CategoriaResponse>
{
    /// <summary>
    /// Valida la existencia del torneo, arma la entidad <see cref="Categoria"/> a partir del comando,
    /// la guarda y devuelve su representación de respuesta.
    /// </summary>
    public async Task<CategoriaResponse> Handle(CreateCategoriaCommand request, CancellationToken ct)
    {
        var torneo = await torneoRepo.GetByIdAsync(request.TorneoId, ct)
            ?? throw new NotFoundException(nameof(Torneo), request.TorneoId);

        if (torneo.Estado == EstadoTorneo.Finalizado)
            throw new ConflictException("No se pueden agregar categorías a un torneo finalizado.");

        var tipoCompetencia = Enum.Parse<TipoCompetencia>(request.TipoCompetencia);
        // El peso solo aplica a Combate; en Formas se descarta cualquier valor recibido.
        var esFormas = tipoCompetencia == TipoCompetencia.Formas;

        var categoria = new Categoria
        {
            Id = Guid.NewGuid(),
            TorneoId = torneo.Id,
            Nombre = request.Nombre,
            TipoCompetencia = tipoCompetencia,
            Sexo = Enum.Parse<Sexo>(request.Sexo),
            RangoEdadMin = request.RangoEdadMin,
            RangoEdadMax = request.RangoEdadMax,
            RangoPesoMin = esFormas ? null : request.RangoPesoMin,
            RangoPesoMax = esFormas ? null : request.RangoPesoMax,
            RangoGraduacionMin = request.RangoGraduacionMin,
            RangoGraduacionMax = request.RangoGraduacionMax
        };

        await categoriaRepo.AddAsync(categoria, ct);

        return categoria.Adapt<CategoriaResponse>();
    }
}
