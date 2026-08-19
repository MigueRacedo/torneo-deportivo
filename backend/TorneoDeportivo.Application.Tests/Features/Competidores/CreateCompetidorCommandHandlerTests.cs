using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Competidores.Commands.CreateCompetidor;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Competidores;

public class CreateCompetidorCommandHandlerTests
{
    private static CreateCompetidorCommand Comando(Guid torneoId) => new(
        torneoId, "Juan", "Pérez", "M", 15, "CinturonVerde",
        45.5m, 1.65m, "Escuela Central", "María Pérez", "1122334455");

    [Fact]
    public async Task Handle_DatosValidos_CargaCompetidorSinCategoria()
    {
        var torneoId = Guid.NewGuid();
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = torneoId, Estado = EstadoTorneo.Borrador });
        var comp = Substitute.For<ICompetidorRepository>();
        var handler = new CreateCompetidorCommandHandler(comp, tor);

        var result = await handler.Handle(Comando(torneoId), CancellationToken.None);

        Assert.Equal("Juan Pérez", result.NombreCompleto);
        Assert.Equal("M", result.Sexo);
        Assert.Null(result.CategoriaId);
        Assert.Null(result.CategoriaNombre);
        await comp.Received(1).AddAsync(
            Arg.Is<Competidor>(c => c.Nombre == "Juan" && c.TorneoId == torneoId
                && c.CategoriaId == null && c.Sexo == Sexo.M),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Torneo?)null);
        var comp = Substitute.For<ICompetidorRepository>();
        var handler = new CreateCompetidorCommandHandler(comp, tor);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(Comando(Guid.NewGuid()), CancellationToken.None));
        await comp.DidNotReceive().AddAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoFinalizado_LanzaConflictException()
    {
        var torneoId = Guid.NewGuid();
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = torneoId, Estado = EstadoTorneo.Finalizado });
        var comp = Substitute.For<ICompetidorRepository>();
        var handler = new CreateCompetidorCommandHandler(comp, tor);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(Comando(torneoId), CancellationToken.None));
        await comp.DidNotReceive().AddAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }
}
