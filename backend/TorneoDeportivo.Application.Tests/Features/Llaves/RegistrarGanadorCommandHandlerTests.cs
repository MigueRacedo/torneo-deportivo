using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Common.Interfaces;
using TorneoDeportivo.Application.Features.Llaves.Commands.RegistrarGanador;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Llaves;

public class RegistrarGanadorCommandHandlerTests
{
    private readonly Guid _torneoId = Guid.NewGuid();
    private readonly Guid _categoriaId = Guid.NewGuid();
    private readonly Guid _comp1 = Guid.NewGuid();
    private readonly Guid _comp2 = Guid.NewGuid();

    private (LlaveCompetencia match, LlaveCompetencia siguiente) Llaves()
    {
        var match = new LlaveCompetencia
        {
            Id = Guid.NewGuid(),
            CategoriaId = _categoriaId,
            Ronda = 1,
            Posicion = 0,
            Competidor1Id = _comp1,
            Competidor2Id = _comp2,
            Competidor1 = new Competidor { Id = _comp1, Nombre = "Ana", Apellido = "Gómez" },
            Competidor2 = new Competidor { Id = _comp2, Nombre = "Bruno", Apellido = "Díaz" },
            Estado = EstadoLlave.Pendiente
        };
        var siguiente = new LlaveCompetencia
        {
            Id = Guid.NewGuid(),
            CategoriaId = _categoriaId,
            Ronda = 2,
            Posicion = 0,
            Estado = EstadoLlave.Pendiente
        };
        return (match, siguiente);
    }

    private (ILlaveCompetenciaRepository llave, ICategoriaRepository cat, ITorneoRepository tor, IBracketNotifier notif)
        Deps(LlaveCompetencia match, LlaveCompetencia siguiente, Guid? categoriaTorneoId = null,
             EstadoTorneo estadoTorneo = EstadoTorneo.Activo)
    {
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(_torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = _torneoId, Estado = estadoTorneo });

        var llave = Substitute.For<ILlaveCompetenciaRepository>();
        llave.GetByIdAsync(match.Id, Arg.Any<CancellationToken>()).Returns(match);
        llave.GetByCategoriaIdAsync(_categoriaId, Arg.Any<CancellationToken>()).Returns([match, siguiente]);
        var cat = Substitute.For<ICategoriaRepository>();
        cat.GetByIdAsync(_categoriaId, Arg.Any<CancellationToken>())
            .Returns(new Categoria { Id = _categoriaId, TorneoId = categoriaTorneoId ?? _torneoId, Nombre = "Cadetes" });
        return (llave, cat, tor, Substitute.For<IBracketNotifier>());
    }

    [Fact]
    public async Task Handle_GanadorValido_RegistraYPropagaYNotifica()
    {
        var (match, siguiente) = Llaves();
        var (llave, cat, tor, notif) = Deps(match, siguiente);
        var handler = new RegistrarGanadorCommandHandler(llave, cat, tor, notif);

        var result = await handler.Handle(new RegistrarGanadorCommand(_torneoId, match.Id, _comp1), CancellationToken.None);

        Assert.Equal(EstadoLlave.Finalizado.ToString(), result.Estado);
        Assert.Equal(_comp1, result.Ganador!.Id);
        Assert.Equal(_comp1, siguiente.Competidor1Id); // pos 0 → slot competidor1 del match siguiente
        // Ambos matches se persisten en una sola operación: el actual y el de la ronda siguiente.
        await llave.Received(1).UpdateRangeAsync(
            Arg.Is<IEnumerable<LlaveCompetencia>>(ls => ls.Contains(match) && ls.Contains(siguiente)),
            Arg.Any<CancellationToken>());
        await notif.Received(1).NotificarMatchActualizadoAsync(_torneoId, _categoriaId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MatchSinLosDosCompetidores_LanzaConflictException()
    {
        // El rival todavía no salió de la ronda anterior: no hay enfrentamiento que resolver.
        var (match, siguiente) = Llaves();
        match.Competidor2Id = null;
        match.Competidor2 = null;
        var (llave, cat, tor, notif) = Deps(match, siguiente);
        var handler = new RegistrarGanadorCommandHandler(llave, cat, tor, notif);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new RegistrarGanadorCommand(_torneoId, match.Id, _comp1), CancellationToken.None));
        await notif.DidNotReceive().NotificarMatchActualizadoAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MatchYaFinalizado_LanzaConflictException()
    {
        // Reabrir un match resuelto dejaría al perdedor propagado aguas abajo.
        var (match, siguiente) = Llaves();
        match.Estado = EstadoLlave.Finalizado;
        match.GanadorId = _comp1;
        var (llave, cat, tor, notif) = Deps(match, siguiente);
        var handler = new RegistrarGanadorCommandHandler(llave, cat, tor, notif);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new RegistrarGanadorCommand(_torneoId, match.Id, _comp2), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_MatchConBye_LanzaConflictException()
    {
        var (match, siguiente) = Llaves();
        match.Estado = EstadoLlave.Bye;
        match.Competidor2Id = null;
        match.Competidor2 = null;
        match.GanadorId = _comp1;
        var (llave, cat, tor, notif) = Deps(match, siguiente);
        var handler = new RegistrarGanadorCommandHandler(llave, cat, tor, notif);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new RegistrarGanadorCommand(_torneoId, match.Id, _comp1), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_GanadorNoEsDelMatch_LanzaConflictException()
    {
        var (match, siguiente) = Llaves();
        var (llave, cat, tor, notif) = Deps(match, siguiente);
        var handler = new RegistrarGanadorCommandHandler(llave, cat, tor, notif);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new RegistrarGanadorCommand(_torneoId, match.Id, Guid.NewGuid()), CancellationToken.None));
        await notif.DidNotReceive().NotificarMatchActualizadoAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_MatchInexistente_LanzaNotFoundException()
    {
        // El torneo existe y está Activo: así el 404 es por el match y no por el guard de estado.
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(_torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = _torneoId, Estado = EstadoTorneo.Activo });
        var llave = Substitute.For<ILlaveCompetenciaRepository>();
        llave.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((LlaveCompetencia?)null);
        var handler = new RegistrarGanadorCommandHandler(
            llave, Substitute.For<ICategoriaRepository>(), tor, Substitute.For<IBracketNotifier>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new RegistrarGanadorCommand(_torneoId, Guid.NewGuid(), _comp1), CancellationToken.None));
    }

    [Theory]
    [InlineData(EstadoTorneo.Borrador)]
    [InlineData(EstadoTorneo.Finalizado)]
    public async Task Handle_TorneoQueNoEstaActivo_LanzaConflictException(EstadoTorneo estado)
    {
        // En Borrador todavía se planifica (y las llaves se pueden rehacer); en Finalizado no se escribe.
        var (match, siguiente) = Llaves();
        var (llave, cat, tor, notif) = Deps(match, siguiente, estadoTorneo: estado);
        var handler = new RegistrarGanadorCommandHandler(llave, cat, tor, notif);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new RegistrarGanadorCommand(_torneoId, match.Id, _comp1), CancellationToken.None));
        await llave.DidNotReceive().UpdateRangeAsync(Arg.Any<IEnumerable<LlaveCompetencia>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PrimerGanador_DejaLaCategoriaEnCurso()
    {
        var (match, siguiente) = Llaves();
        var (llave, cat, tor, notif) = Deps(match, siguiente);
        var categoria = await cat.GetByIdAsync(_categoriaId, CancellationToken.None);
        var handler = new RegistrarGanadorCommandHandler(llave, cat, tor, notif);

        await handler.Handle(new RegistrarGanadorCommand(_torneoId, match.Id, _comp1), CancellationToken.None);

        // Queda un match pendiente (el de la ronda 2), así que la categoría está compitiendo, no terminada.
        Assert.Equal(EstadoCategoria.EnCurso, categoria!.Estado);
        await cat.Received(1).UpdateAsync(categoria, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UltimoMatchResuelto_DejaLaCategoriaFinalizada()
    {
        // Un bracket de un solo match: al resolverlo no queda nada pendiente.
        var (match, _) = Llaves();
        var llave = Substitute.For<ILlaveCompetenciaRepository>();
        llave.GetByIdAsync(match.Id, Arg.Any<CancellationToken>()).Returns(match);
        llave.GetByCategoriaIdAsync(_categoriaId, Arg.Any<CancellationToken>()).Returns([match]);
        var categoria = new Categoria { Id = _categoriaId, TorneoId = _torneoId, Nombre = "Cadetes" };
        var cat = Substitute.For<ICategoriaRepository>();
        cat.GetByIdAsync(_categoriaId, Arg.Any<CancellationToken>()).Returns(categoria);
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(_torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = _torneoId, Estado = EstadoTorneo.Activo });
        var handler = new RegistrarGanadorCommandHandler(llave, cat, tor, Substitute.For<IBracketNotifier>());

        await handler.Handle(new RegistrarGanadorCommand(_torneoId, match.Id, _comp1), CancellationToken.None);

        Assert.Equal(EstadoCategoria.Finalizada, categoria.Estado);
    }

    [Fact]
    public async Task Handle_MatchDeOtroTorneo_LanzaNotFoundException()
    {
        var (match, siguiente) = Llaves();
        var (llave, cat, tor, notif) = Deps(match, siguiente, categoriaTorneoId: Guid.NewGuid());
        var handler = new RegistrarGanadorCommandHandler(llave, cat, tor, notif);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new RegistrarGanadorCommand(_torneoId, match.Id, _comp1), CancellationToken.None));
    }
}
