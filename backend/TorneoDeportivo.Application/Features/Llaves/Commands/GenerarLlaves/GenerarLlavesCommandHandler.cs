using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Llaves.Commands.GenerarLlaves;

/// <summary>
/// Handler CQRS de la generación de llaves: clasifica a los competidores sin categoría en la primera categoría
/// que encaje por sus atributos y, para cada categoría con 2+ competidores que aún no tenga llaves, genera el
/// bracket de eliminación directa. No regenera llaves ya existentes.
/// </summary>
public class GenerarLlavesCommandHandler(
    ITorneoRepository torneoRepo,
    ICategoriaRepository categoriaRepo,
    ICompetidorRepository competidorRepo,
    ILlaveCompetenciaRepository llaveRepo) : IRequestHandler<GenerarLlavesCommand, GenerarLlavesResponse>
{
    /// <summary>
    /// Verifica el torneo, clasifica los competidores sin categoría, genera las llaves por categoría elegible
    /// y devuelve un resumen del resultado.
    /// </summary>
    public async Task<GenerarLlavesResponse> Handle(GenerarLlavesCommand request, CancellationToken ct)
    {
        var torneo = await torneoRepo.GetByIdAsync(request.TorneoId, ct)
            ?? throw new NotFoundException(nameof(Torneo), request.TorneoId);

        var categorias = await categoriaRepo.GetByTorneoIdAsync(torneo.Id, ct);
        var competidores = await competidorRepo.GetByTorneoIdAsync(torneo.Id, ct);

        // 1. Clasificación: cada competidor sin categoría va a la primera categoría que encaje por sus atributos.
        // Solo se consideran categorías SIN llaves generadas: asignarlo a una cuyo bracket ya está armado lo
        // dejaría con categoría pero fuera de la llave, y además invisible (no contaría como "sin clasificar"
        // ni aparecería en la lista de competidores sin categoría del frontend).
        var clasificables = categorias.Where(cat => !cat.LlavesGeneradas).ToList();
        var asignados = new List<Competidor>();
        foreach (var comp in competidores.Where(c => c.CategoriaId is null))
        {
            var categoria = clasificables.FirstOrDefault(cat => ClasificadorCompetidores.Encaja(comp, cat));
            if (categoria is not null)
            {
                comp.CategoriaId = categoria.Id;
                asignados.Add(comp);
            }
        }
        if (asignados.Count > 0)
            await competidorRepo.UpdateRangeAsync(asignados, ct);

        // 2. Generación por categoría: 2+ competidores y sin llaves previas (regla de no regenerar).
        var resultados = new List<GenerarLlavesResultado>();
        foreach (var categoria in categorias)
        {
            var deLaCategoria = competidores.Where(c => c.CategoriaId == categoria.Id).ToList();

            if (categoria.LlavesGeneradas)
            {
                resultados.Add(new(categoria.Id, categoria.Nombre, deLaCategoria.Count, true, "Ya tenía llaves generadas."));
                continue;
            }
            if (deLaCategoria.Count < 2)
            {
                resultados.Add(new(categoria.Id, categoria.Nombre, deLaCategoria.Count, false,
                    deLaCategoria.Count == 1 ? "1 competidor (campeón directo)." : "Sin competidores que encajen."));
                continue;
            }

            var llaves = BracketGeneratorService.Generar(categoria.Id, deLaCategoria);
            await llaveRepo.AddRangeAsync(llaves, ct);

            categoria.LlavesGeneradas = true;
            await categoriaRepo.UpdateAsync(categoria, ct);

            resultados.Add(new(categoria.Id, categoria.Nombre, deLaCategoria.Count, true, null));
        }

        var sinClasificar = competidores.Count(c => c.CategoriaId is null);
        return new GenerarLlavesResponse(sinClasificar, resultados);
    }
}
