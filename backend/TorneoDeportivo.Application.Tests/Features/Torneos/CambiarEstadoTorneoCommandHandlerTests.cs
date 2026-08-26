using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Torneos.Commands.CambiarEstadoTorneo;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Torneos;

public class CambiarEstadoTorneoCommandHandlerTests
{
    private readonly Guid _torneoId = Guid.NewGuid();

    private static Categoria Categoria(Guid torneoId, EstadoCategoria estado, string nombre = "Cadetes M") => new()
    {
        Id = Guid.NewGuid(),
        TorneoId = torneoId,
        Nombre = nombre,
        Estado = estado
    };

    private (ITorneoRepository tor, ICategoriaRepository cat, Torneo torneo) Repos(
        EstadoTorneo estadoActual, params Categoria[] categorias)
    {
        var torneo = new Torneo { Id = _torneoId, Nombre = "Copa", Estado = estadoActual };
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(_torneoId, Arg.Any<CancellationToken>()).Returns(torneo);
        var cat = Substitute.For<ICategoriaRepository>();
        cat.GetByTorneoIdAsync(_torneoId, Arg.Any<CancellationToken>()).Returns(categorias.ToList());
        return (tor, cat, torneo);
    }

    // ── Borrador → Activo ────────────────────────────────────────────────

    [Fact]
    public async Task Handle_ActivarConLlavesGeneradas_CambiaElEstado()
    {
        var (tor, cat, torneo) = Repos(EstadoTorneo.Borrador, Categoria(_torneoId, EstadoCategoria.LlavesGeneradas));
        var handler = new CambiarEstadoTorneoCommandHandler(tor, cat);

        var result = await handler.Handle(new CambiarEstadoTorneoCommand(_torneoId, "Activo"), CancellationToken.None);

        Assert.Equal(EstadoTorneo.Activo, torneo.Estado);
        Assert.Equal("Activo", result.Estado);
        await tor.Received(1).UpdateAsync(torneo, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ActivarSinNingunaLlaveGenerada_LanzaConflictException()
    {
        // Activar sin brackets dejaría el torneo trabado: en Activo ya no se pueden generar llaves.
        var (tor, cat, _) = Repos(EstadoTorneo.Borrador, Categoria(_torneoId, EstadoCategoria.SinLlaves));
        var handler = new CambiarEstadoTorneoCommandHandler(tor, cat);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new CambiarEstadoTorneoCommand(_torneoId, "Activo"), CancellationToken.None));
        await tor.DidNotReceive().UpdateAsync(Arg.Any<Torneo>(), Arg.Any<CancellationToken>());
    }

    // ── Activo → Finalizado ──────────────────────────────────────────────

    [Fact]
    public async Task Handle_FinalizarConTodasLasCategoriasResueltas_CambiaElEstado()
    {
        var (tor, cat, torneo) = Repos(EstadoTorneo.Activo,
            Categoria(_torneoId, EstadoCategoria.Finalizada),
            Categoria(_torneoId, EstadoCategoria.SinLlaves, "Sin inscriptos"));
        var handler = new CambiarEstadoTorneoCommandHandler(tor, cat);

        await handler.Handle(new CambiarEstadoTorneoCommand(_torneoId, "Finalizado"), CancellationToken.None);

        // La categoría sin llaves no bloquea: nunca llegó a competir.
        Assert.Equal(EstadoTorneo.Finalizado, torneo.Estado);
    }

    [Fact]
    public async Task Handle_FinalizarConCategoriasEnCurso_LanzaConflictExceptionConSusNombres()
    {
        var (tor, cat, _) = Repos(EstadoTorneo.Activo,
            Categoria(_torneoId, EstadoCategoria.EnCurso, "Cadetes M"),
            Categoria(_torneoId, EstadoCategoria.Finalizada, "Juveniles F"));
        var handler = new CambiarEstadoTorneoCommandHandler(tor, cat);

        var ex = await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new CambiarEstadoTorneoCommand(_torneoId, "Finalizado"), CancellationToken.None));

        // El mensaje tiene que decir cuál falta, no solo que falta algo.
        Assert.Contains("Cadetes M", ex.Message);
        Assert.DoesNotContain("Juveniles F", ex.Message);
    }

    [Fact]
    public async Task Handle_FinalizarForzado_IgnoraLasCategoriasPendientes()
    {
        // Decisión del Coordinador: una categoría puede quedar sin disputarse.
        var (tor, cat, torneo) = Repos(EstadoTorneo.Activo, Categoria(_torneoId, EstadoCategoria.EnCurso));
        var handler = new CambiarEstadoTorneoCommandHandler(tor, cat);

        await handler.Handle(new CambiarEstadoTorneoCommand(_torneoId, "Finalizado", Forzar: true), CancellationToken.None);

        Assert.Equal(EstadoTorneo.Finalizado, torneo.Estado);
    }

    // ── Activo → Borrador ────────────────────────────────────────────────

    [Fact]
    public async Task Handle_VolverABorradorSinResultados_CambiaElEstado()
    {
        var (tor, cat, torneo) = Repos(EstadoTorneo.Activo, Categoria(_torneoId, EstadoCategoria.LlavesGeneradas));
        var handler = new CambiarEstadoTorneoCommandHandler(tor, cat);

        await handler.Handle(new CambiarEstadoTorneoCommand(_torneoId, "Borrador"), CancellationToken.None);

        Assert.Equal(EstadoTorneo.Borrador, torneo.Estado);
    }

    [Theory]
    [InlineData(EstadoCategoria.EnCurso)]
    [InlineData(EstadoCategoria.Finalizada)]
    public async Task Handle_VolverABorradorConResultados_LanzaConflictException(EstadoCategoria estado)
    {
        // Volver a planificación habilitaría rehacer llaves y descartaría los ganadores ya cargados.
        var (tor, cat, _) = Repos(EstadoTorneo.Activo, Categoria(_torneoId, estado));
        var handler = new CambiarEstadoTorneoCommandHandler(tor, cat);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new CambiarEstadoTorneoCommand(_torneoId, "Borrador"), CancellationToken.None));
    }

    // ── Transiciones ilegales ────────────────────────────────────────────

    [Fact]
    public async Task Handle_BorradorAFinalizado_LanzaConflictException()
    {
        // No se saltean estados: un torneo no puede cerrarse sin haber competido.
        var (tor, cat, _) = Repos(EstadoTorneo.Borrador, Categoria(_torneoId, EstadoCategoria.LlavesGeneradas));
        var handler = new CambiarEstadoTorneoCommandHandler(tor, cat);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new CambiarEstadoTorneoCommand(_torneoId, "Finalizado"), CancellationToken.None));
    }

    [Theory]
    [InlineData("Activo")]
    [InlineData("Borrador")]
    public async Task Handle_ReabrirUnTorneoFinalizado_LanzaConflictException(string destino)
    {
        var (tor, cat, _) = Repos(EstadoTorneo.Finalizado, Categoria(_torneoId, EstadoCategoria.Finalizada));
        var handler = new CambiarEstadoTorneoCommandHandler(tor, cat);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new CambiarEstadoTorneoCommand(_torneoId, destino), CancellationToken.None));
    }

    // ── Casos borde ──────────────────────────────────────────────────────

    [Fact]
    public async Task Handle_MismoEstado_EsIdempotenteYNoPersiste()
    {
        var (tor, cat, torneo) = Repos(EstadoTorneo.Activo, Categoria(_torneoId, EstadoCategoria.EnCurso));
        var handler = new CambiarEstadoTorneoCommandHandler(tor, cat);

        var result = await handler.Handle(new CambiarEstadoTorneoCommand(_torneoId, "Activo"), CancellationToken.None);

        Assert.Equal("Activo", result.Estado);
        Assert.Equal(EstadoTorneo.Activo, torneo.Estado);
        await tor.DidNotReceive().UpdateAsync(Arg.Any<Torneo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Torneo?)null);
        var handler = new CambiarEstadoTorneoCommandHandler(tor, Substitute.For<ICategoriaRepository>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new CambiarEstadoTorneoCommand(Guid.NewGuid(), "Activo"), CancellationToken.None));
    }
}
