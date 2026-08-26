using Mapster;
using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.CambiarEstadoTorneo;

/// <summary>
/// Handler CQRS de las transiciones de estado del torneo (H0009): valida que el salto sea legal y que se
/// cumplan las precondiciones de cada uno, y persiste el nuevo estado.
/// </summary>
/// <remarks>
/// Las transiciones son las tres del ciclo de vida y nada más:
/// <list type="bullet">
///   <item>Borrador → Activo: arranca la competencia.</item>
///   <item>Activo → Finalizado: la cierra.</item>
///   <item>Activo → Borrador: vuelve a planificación (solo si todavía no se compitió).</item>
/// </list>
/// No se puede saltar de Borrador a Finalizado ni reabrir un torneo cerrado.
/// </remarks>
public class CambiarEstadoTorneoCommandHandler(
    ITorneoRepository torneoRepo,
    ICategoriaRepository categoriaRepo) : IRequestHandler<CambiarEstadoTorneoCommand, TorneoResponse>
{
    /// <summary>
    /// Busca el torneo (404), verifica que la transición pedida sea válida (409) y guarda el nuevo estado.
    /// Si el torneo ya estaba en ese estado, no hace nada y devuelve el torneo tal cual.
    /// </summary>
    public async Task<TorneoResponse> Handle(CambiarEstadoTorneoCommand request, CancellationToken ct)
    {
        var torneo = await torneoRepo.GetByIdAsync(request.TorneoId, ct)
            ?? throw new NotFoundException(nameof(Torneo), request.TorneoId);

        var destino = Enum.Parse<EstadoTorneo>(request.NuevoEstado);

        // Idempotente: pedir el estado en el que ya está no es un error.
        if (torneo.Estado == destino)
            return torneo.Adapt<TorneoResponse>();

        var categorias = await categoriaRepo.GetByTorneoIdAsync(torneo.Id, ct);

        switch (torneo.Estado, destino)
        {
            case (EstadoTorneo.Borrador, EstadoTorneo.Activo):
                ValidarInicio(categorias);
                break;

            case (EstadoTorneo.Activo, EstadoTorneo.Finalizado):
                ValidarCierre(categorias, request.Forzar);
                break;

            case (EstadoTorneo.Activo, EstadoTorneo.Borrador):
                ValidarVueltaAPlanificacion(categorias);
                break;

            default:
                throw new ConflictException(
                    $"No se puede pasar de {torneo.Estado} a {destino}. " +
                    "El torneo va de Borrador a Activo y de Activo a Finalizado; " +
                    "solo se puede volver a Borrador desde Activo y sin resultados cargados.");
        }

        torneo.Estado = destino;
        await torneoRepo.UpdateAsync(torneo, ct);

        return torneo.Adapt<TorneoResponse>();
    }

    /// <summary>
    /// Un torneo no puede arrancar sin al menos un bracket armado: no habría nada que competir y el
    /// estado Activo bloquearía la generación de llaves, dejándolo trabado.
    /// </summary>
    private static void ValidarInicio(List<Categoria> categorias)
    {
        if (!categorias.Any(c => c.LlavesGeneradas))
            throw new ConflictException(
                "Para activar el torneo hace falta al menos una categoría con llaves generadas. " +
                "Generá las llaves antes de arrancar.");
    }

    /// <summary>
    /// Cerrar el torneo exige que todas las categorías con llaves estén resueltas, salvo que el
    /// Coordinador lo fuerce (una categoría puede quedar sin disputarse).
    /// </summary>
    private static void ValidarCierre(List<Categoria> categorias, bool forzar)
    {
        if (forzar) return;

        var pendientes = categorias
            .Where(c => c.LlavesGeneradas && c.Estado != EstadoCategoria.Finalizada)
            .ToList();

        if (pendientes.Count > 0)
            throw new ConflictException(
                $"Quedan {pendientes.Count} categoría(s) sin terminar: " +
                $"{string.Join(", ", pendientes.Select(c => c.Nombre))}. " +
                "Registrá los ganadores que faltan o finalizá el torneo de todos modos.");
    }

    /// <summary>
    /// Volver a planificación solo es seguro mientras no se haya registrado ningún resultado: si ya se
    /// compitió, permitirlo habilitaría rehacer llaves y descartar en silencio los ganadores cargados.
    /// </summary>
    private static void ValidarVueltaAPlanificacion(List<Categoria> categorias)
    {
        if (categorias.Any(c => c.Estado >= EstadoCategoria.EnCurso))
            throw new ConflictException(
                "No se puede volver a Borrador: ya hay resultados registrados. " +
                "Volver descartaría los ganadores cargados.");
    }
}
