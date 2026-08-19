using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Torneos.Commands.DeleteTorneo;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Torneos;

public class DeleteTorneoCommandHandlerTests
{
    [Fact]
    public async Task Handle_TorneoEnBorrador_LoElimina()
    {
        var id = Guid.NewGuid();
        var torneo = new Torneo { Id = id, Estado = EstadoTorneo.Borrador };
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(torneo);
        var handler = new DeleteTorneoCommandHandler(repo);

        await handler.Handle(new DeleteTorneoCommand(id), CancellationToken.None);

        await repo.Received(1).DeleteAsync(torneo, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Torneo?)null);
        var handler = new DeleteTorneoCommandHandler(repo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new DeleteTorneoCommand(Guid.NewGuid()), CancellationToken.None));
        await repo.DidNotReceive().DeleteAsync(Arg.Any<Torneo>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(EstadoTorneo.Activo)]
    [InlineData(EstadoTorneo.Finalizado)]
    public async Task Handle_TorneoNoBorrador_LanzaConflictException(EstadoTorneo estado)
    {
        var id = Guid.NewGuid();
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(new Torneo { Id = id, Estado = estado });
        var handler = new DeleteTorneoCommandHandler(repo);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new DeleteTorneoCommand(id), CancellationToken.None));
        await repo.DidNotReceive().DeleteAsync(Arg.Any<Torneo>(), Arg.Any<CancellationToken>());
    }
}
