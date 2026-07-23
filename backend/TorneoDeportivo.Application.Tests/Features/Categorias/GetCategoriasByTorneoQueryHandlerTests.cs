using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Categorias.Queries.GetCategoriasByTorneo;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Categorias;

public class GetCategoriasByTorneoQueryHandlerTests
{
    [Fact]
    public async Task Handle_TorneoExistente_RetornaCategoriasMapeadas()
    {
        var torneoId = Guid.NewGuid();
        var torneoRepo = Substitute.For<ITorneoRepository>();
        torneoRepo.GetByIdAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = torneoId });
        var categoriaRepo = Substitute.For<ICategoriaRepository>();
        categoriaRepo.GetByTorneoIdAsync(torneoId, Arg.Any<CancellationToken>()).Returns(
        [
            new Categoria
            {
                Id = Guid.NewGuid(), TorneoId = torneoId, Nombre = "Infantil A",
                TipoCompetencia = TipoCompetencia.Formas, Sexo = Sexo.M,
                RangoGraduacionMin = "CinturonBlanco", RangoGraduacionMax = "CinturonAmarillo"
            }
        ]);
        var handler = new GetCategoriasByTorneoQueryHandler(categoriaRepo, torneoRepo);

        var result = await handler.Handle(new GetCategoriasByTorneoQuery(torneoId), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Infantil A", result[0].Nombre);
        Assert.Equal("Formas", result[0].TipoCompetencia);
        Assert.Equal("M", result[0].Sexo);
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var torneoRepo = Substitute.For<ITorneoRepository>();
        torneoRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Torneo?)null);
        var categoriaRepo = Substitute.For<ICategoriaRepository>();
        var handler = new GetCategoriasByTorneoQueryHandler(categoriaRepo, torneoRepo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetCategoriasByTorneoQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
