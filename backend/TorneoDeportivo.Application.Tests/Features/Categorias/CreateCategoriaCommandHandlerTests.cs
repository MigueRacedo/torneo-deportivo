using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Categorias.Commands.CreateCategoria;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Categorias;

public class CreateCategoriaCommandHandlerTests
{
    private static CreateCategoriaCommand CommandValido(Guid torneoId) => new(
        torneoId,
        "Adultos A - Combate Femenino",
        "Combate",
        "F",
        18, 35,
        50m, 55m,
        "CinturonAmarillo", "CinturonVerde");

    [Fact]
    public async Task Handle_TorneoExistente_CreaCategoriaYRetornaResponse()
    {
        var torneoId = Guid.NewGuid();
        var torneoRepo = Substitute.For<ITorneoRepository>();
        torneoRepo.GetByIdAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = torneoId, Nombre = "Copa Apertura" });
        var categoriaRepo = Substitute.For<ICategoriaRepository>();
        var handler = new CreateCategoriaCommandHandler(categoriaRepo, torneoRepo);

        var result = await handler.Handle(CommandValido(torneoId), CancellationToken.None);

        Assert.Equal(torneoId, result.TorneoId);
        Assert.Equal("Adultos A - Combate Femenino", result.Nombre);
        Assert.Equal("Combate", result.TipoCompetencia);
        Assert.Equal("F", result.Sexo);
        await categoriaRepo.Received(1).AddAsync(
            Arg.Is<Categoria>(c => c.TorneoId == torneoId
                && c.TipoCompetencia == TipoCompetencia.Combate
                && c.Sexo == Sexo.F),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var torneoRepo = Substitute.For<ITorneoRepository>();
        torneoRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Torneo?)null);
        var categoriaRepo = Substitute.For<ICategoriaRepository>();
        var handler = new CreateCategoriaCommandHandler(categoriaRepo, torneoRepo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(CommandValido(Guid.NewGuid()), CancellationToken.None));
        await categoriaRepo.DidNotReceive().AddAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }
}
