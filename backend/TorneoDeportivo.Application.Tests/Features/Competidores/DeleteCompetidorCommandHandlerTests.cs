using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Competidores.Commands.DeleteCompetidor;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Competidores;

public class DeleteCompetidorCommandHandlerTests
{
    private static Competidor CompetidorExistente(Guid id, Guid torneoId) => new()
    {
        Id = id,
        TorneoId = torneoId,
        Nombre = "Juan",
        Apellido = "Pérez"
    };

    private static ITorneoRepository TorneoRepoCon(Guid torneoId, EstadoTorneo estado)
    {
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = torneoId, Estado = estado });
        return repo;
    }

    [Fact]
    public async Task Handle_CompetidorExistente_LoElimina()
    {
        var id = Guid.NewGuid();
        var torneoId = Guid.NewGuid();
        var comp = Substitute.For<ICompetidorRepository>();
        var competidor = CompetidorExistente(id, torneoId);
        comp.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(competidor);
        var handler = new DeleteCompetidorCommandHandler(comp, TorneoRepoCon(torneoId, EstadoTorneo.Borrador));

        await handler.Handle(new DeleteCompetidorCommand(id, torneoId), CancellationToken.None);

        await comp.Received(1).DeleteAsync(competidor, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CompetidorInexistente_LanzaNotFoundException()
    {
        var comp = Substitute.For<ICompetidorRepository>();
        comp.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Competidor?)null);
        var handler = new DeleteCompetidorCommandHandler(comp, Substitute.For<ITorneoRepository>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new DeleteCompetidorCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
        await comp.DidNotReceive().DeleteAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CompetidorDeOtroTorneo_LanzaNotFoundException()
    {
        var id = Guid.NewGuid();
        var comp = Substitute.For<ICompetidorRepository>();
        comp.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CompetidorExistente(id, Guid.NewGuid()));
        var handler = new DeleteCompetidorCommandHandler(comp, Substitute.For<ITorneoRepository>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new DeleteCompetidorCommand(id, Guid.NewGuid()), CancellationToken.None));
        await comp.DidNotReceive().DeleteAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoFinalizado_LanzaConflictException()
    {
        var id = Guid.NewGuid();
        var torneoId = Guid.NewGuid();
        var comp = Substitute.For<ICompetidorRepository>();
        comp.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CompetidorExistente(id, torneoId));
        var handler = new DeleteCompetidorCommandHandler(comp, TorneoRepoCon(torneoId, EstadoTorneo.Finalizado));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new DeleteCompetidorCommand(id, torneoId), CancellationToken.None));
        await comp.DidNotReceive().DeleteAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }
}
