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
    private static Categoria Categoria(Guid torneoId, string nombre = "Infantil A") => new()
    {
        Id = Guid.NewGuid(),
        TorneoId = torneoId,
        Nombre = nombre,
        TipoCompetencia = TipoCompetencia.Formas,
        Sexo = Sexo.M,
        RangoGraduacionMin = "CinturonBlanco",
        RangoGraduacionMax = "CinturonAmarillo"
    };

    private static (ICategoriaRepository cat, ICompetidorRepository comp, ITorneoRepository tor) Repos(
        Guid torneoId, List<Categoria>? categorias = null, Dictionary<Guid, int>? conteo = null)
    {
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(torneoId, Arg.Any<CancellationToken>()).Returns(new Torneo { Id = torneoId });
        var cat = Substitute.For<ICategoriaRepository>();
        cat.GetByTorneoIdAsync(torneoId, Arg.Any<CancellationToken>()).Returns(categorias ?? []);
        var comp = Substitute.For<ICompetidorRepository>();
        comp.GetConteoPorCategoriaAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(conteo ?? []);
        return (cat, comp, tor);
    }

    [Fact]
    public async Task Handle_TorneoExistente_RetornaCategoriasMapeadas()
    {
        var torneoId = Guid.NewGuid();
        var (cat, comp, tor) = Repos(torneoId, [Categoria(torneoId)]);
        var handler = new GetCategoriasByTorneoQueryHandler(cat, comp, tor);

        var result = await handler.Handle(new GetCategoriasByTorneoQuery(torneoId), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Infantil A", result[0].Nombre);
        Assert.Equal("Formas", result[0].TipoCompetencia);
        Assert.Equal("M", result[0].Sexo);
    }

    [Fact]
    public async Task Handle_ConCompetidoresAsignados_DevuelveElTotalPorCategoria()
    {
        var torneoId = Guid.NewGuid();
        var conAlumnos = Categoria(torneoId, "Cadetes M");
        var vacia = Categoria(torneoId, "Juveniles F");
        var (cat, comp, tor) = Repos(torneoId, [conAlumnos, vacia],
            new Dictionary<Guid, int> { [conAlumnos.Id] = 7 });
        var handler = new GetCategoriasByTorneoQueryHandler(cat, comp, tor);

        var result = await handler.Handle(new GetCategoriasByTorneoQuery(torneoId), CancellationToken.None);

        Assert.Equal(7, result.First(c => c.Id == conAlumnos.Id).TotalCompetidores);
        // Una categoría sin competidores no aparece en el diccionario: debe informar 0, no null.
        Assert.Equal(0, result.First(c => c.Id == vacia.Id).TotalCompetidores);
    }

    [Fact]
    public async Task Handle_ConteoSeConsultaUnaSolaVezParaTodasLasCategorias()
    {
        var torneoId = Guid.NewGuid();
        var (cat, comp, tor) = Repos(torneoId, [Categoria(torneoId, "A"), Categoria(torneoId, "B"), Categoria(torneoId, "C")]);
        var handler = new GetCategoriasByTorneoQueryHandler(cat, comp, tor);

        await handler.Handle(new GetCategoriasByTorneoQuery(torneoId), CancellationToken.None);

        // Un único GROUP BY, no una consulta de conteo por categoría (evita el N+1).
        await comp.Received(1).GetConteoPorCategoriaAsync(torneoId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var torneoRepo = Substitute.For<ITorneoRepository>();
        torneoRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Torneo?)null);
        var handler = new GetCategoriasByTorneoQueryHandler(
            Substitute.For<ICategoriaRepository>(), Substitute.For<ICompetidorRepository>(), torneoRepo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetCategoriasByTorneoQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
