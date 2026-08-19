using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Competidores.Commands.UpdateCompetidor;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Competidores;

public class UpdateCompetidorCommandHandlerTests
{
    private static UpdateCompetidorCommand Comando(Guid id, Guid torneoId) => new(
        id, torneoId, "Ana", "Gómez", "F", 16, "CinturonAzul",
        52m, 1.70m, "Escuela Norte", "Luis Gómez", null);

    private static Competidor CompetidorExistente(Guid id, Guid torneoId) => new()
    {
        Id = id,
        TorneoId = torneoId,
        Nombre = "Juan",
        Apellido = "Pérez",
        Sexo = Sexo.M,
        Edad = 15,
        Graduacion = "CinturonVerde",
        Peso = 45m,
        Altura = 1.65m,
        Escuela = "Escuela Central",
        Responsable = "María Pérez"
    };

    private static ITorneoRepository TorneoRepoCon(Guid torneoId, EstadoTorneo estado)
    {
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = torneoId, Estado = estado });
        return repo;
    }

    [Fact]
    public async Task Handle_CompetidorExistente_ActualizaYRetornaResponse()
    {
        var id = Guid.NewGuid();
        var torneoId = Guid.NewGuid();
        var comp = Substitute.For<ICompetidorRepository>();
        comp.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CompetidorExistente(id, torneoId));
        var handler = new UpdateCompetidorCommandHandler(comp, TorneoRepoCon(torneoId, EstadoTorneo.Borrador));

        var result = await handler.Handle(Comando(id, torneoId), CancellationToken.None);

        Assert.Equal("Ana Gómez", result.NombreCompleto);
        Assert.Equal("F", result.Sexo);
        Assert.Equal(16, result.Edad);
        await comp.Received(1).UpdateAsync(
            Arg.Is<Competidor>(c => c.Id == id && c.Apellido == "Gómez" && c.Sexo == Sexo.F),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CompetidorInexistente_LanzaNotFoundException()
    {
        var comp = Substitute.For<ICompetidorRepository>();
        comp.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Competidor?)null);
        var handler = new UpdateCompetidorCommandHandler(comp, Substitute.For<ITorneoRepository>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(Comando(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
        await comp.DidNotReceive().UpdateAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CompetidorDeOtroTorneo_LanzaNotFoundException()
    {
        var id = Guid.NewGuid();
        var comp = Substitute.For<ICompetidorRepository>();
        comp.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CompetidorExistente(id, Guid.NewGuid()));
        var handler = new UpdateCompetidorCommandHandler(comp, Substitute.For<ITorneoRepository>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(Comando(id, Guid.NewGuid()), CancellationToken.None));
        await comp.DidNotReceive().UpdateAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoFinalizado_LanzaConflictException()
    {
        var id = Guid.NewGuid();
        var torneoId = Guid.NewGuid();
        var comp = Substitute.For<ICompetidorRepository>();
        comp.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CompetidorExistente(id, torneoId));
        var handler = new UpdateCompetidorCommandHandler(comp, TorneoRepoCon(torneoId, EstadoTorneo.Finalizado));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(Comando(id, torneoId), CancellationToken.None));
        await comp.DidNotReceive().UpdateAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }
}
