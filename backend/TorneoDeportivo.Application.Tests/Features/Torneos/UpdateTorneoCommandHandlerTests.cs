using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Torneos.Commands.UpdateTorneo;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Torneos;

public class UpdateTorneoCommandHandlerTests
{
    private static UpdateTorneoCommand Comando(Guid id) => new(
        id, "Copa Clausura", DateOnly.FromDateTime(DateTime.Today.AddDays(20)), "Club Nuevo", null);

    [Fact]
    public async Task Handle_TorneoExistente_ActualizaYRetornaResponse()
    {
        var id = Guid.NewGuid();
        var torneo = new Torneo
        {
            Id = id,
            Nombre = "Copa Apertura",
            Fecha = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            Lugar = "Club Viejo",
            Estado = EstadoTorneo.Borrador
        };
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(torneo);
        var handler = new UpdateTorneoCommandHandler(repo);

        var result = await handler.Handle(Comando(id), CancellationToken.None);

        Assert.Equal("Copa Clausura", result.Nombre);
        Assert.Equal("Club Nuevo", result.Lugar);
        await repo.Received(1).UpdateAsync(
            Arg.Is<Torneo>(t => t.Id == id && t.Nombre == "Copa Clausura" && t.Lugar == "Club Nuevo"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Torneo?)null);
        var handler = new UpdateTorneoCommandHandler(repo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(Comando(Guid.NewGuid()), CancellationToken.None));
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Torneo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoFinalizado_LanzaConflictException()
    {
        var id = Guid.NewGuid();
        var torneo = new Torneo { Id = id, Nombre = "Copa Vieja", Estado = EstadoTorneo.Finalizado };
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(torneo);
        var handler = new UpdateTorneoCommandHandler(repo);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(Comando(id), CancellationToken.None));
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Torneo>(), Arg.Any<CancellationToken>());
    }
}
