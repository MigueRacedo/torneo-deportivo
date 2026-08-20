using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Llaves.Queries.GetBracket;

/// <summary>
/// Handler CQRS que devuelve el bracket de una categoría: valida que la categoría exista y pertenezca al torneo,
/// carga sus llaves y las agrupa por ronda.
/// </summary>
public class GetBracketQueryHandler(
    ICategoriaRepository categoriaRepo,
    ILlaveCompetenciaRepository llaveRepo) : IRequestHandler<GetBracketQuery, BracketResponse>
{
    /// <summary>
    /// Busca la categoría (404 si no existe o no pertenece al torneo), obtiene sus llaves y las mapea a
    /// <see cref="BracketResponse"/>.
    /// </summary>
    public async Task<BracketResponse> Handle(GetBracketQuery request, CancellationToken ct)
    {
        var categoria = await categoriaRepo.GetByIdAsync(request.CategoriaId, ct);
        if (categoria is null || categoria.TorneoId != request.TorneoId)
            throw new NotFoundException(nameof(Categoria), request.CategoriaId);

        var llaves = await llaveRepo.GetByCategoriaIdAsync(categoria.Id, ct);
        return llaves.ToBracketResponse(categoria);
    }
}
