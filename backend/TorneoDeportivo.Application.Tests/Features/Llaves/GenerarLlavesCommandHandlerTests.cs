using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Llaves.Commands.GenerarLlaves;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Llaves;

public class GenerarLlavesCommandHandlerTests
{
    private static Categoria Categoria(Guid torneoId) => new()
    {
        Id = Guid.NewGuid(),
        TorneoId = torneoId,
        Nombre = "Cadetes Masculino",
        Sexo = Sexo.M,
        RangoEdadMin = 12,
        RangoEdadMax = 14,
        RangoGraduacionMin = "CinturonBlanco",
        RangoGraduacionMax = "CinturonVerde",
        LlavesGeneradas = false
    };

    private static Competidor Competidor(Guid torneoId, Sexo sexo = Sexo.M, int edad = 13,
        string graduacion = "CinturonAmarillo") => new()
    {
        Id = Guid.NewGuid(),
        TorneoId = torneoId,
        CategoriaId = null,
        Nombre = "N",
        Apellido = "A",
        Sexo = sexo,
        Edad = edad,
        Graduacion = graduacion,
        Peso = 40m
    };

    private static (ITorneoRepository tor, ICategoriaRepository cat, ICompetidorRepository comp, ILlaveCompetenciaRepository llave)
        Repos(Guid torneoId, List<Categoria> categorias, List<Competidor> competidores)
    {
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = torneoId, Estado = EstadoTorneo.Borrador });
        var cat = Substitute.For<ICategoriaRepository>();
        cat.GetByTorneoIdAsync(torneoId, Arg.Any<CancellationToken>()).Returns(categorias);
        var comp = Substitute.For<ICompetidorRepository>();
        comp.GetByTorneoIdAsync(torneoId, Arg.Any<CancellationToken>()).Returns(competidores);
        return (tor, cat, comp, Substitute.For<ILlaveCompetenciaRepository>());
    }

    [Fact]
    public async Task Handle_DosCompetidoresQueEncajan_ClasificaYGeneraLlaves()
    {
        var torneoId = Guid.NewGuid();
        var categoria = Categoria(torneoId);
        var competidores = new List<Competidor> { Competidor(torneoId), Competidor(torneoId) };
        var (tor, cat, comp, llave) = Repos(torneoId, [categoria], competidores);
        var handler = new GenerarLlavesCommandHandler(tor, cat, comp, llave);

        var result = await handler.Handle(new GenerarLlavesCommand(torneoId), CancellationToken.None);

        // Los competidores quedaron clasificados en la categoría.
        Assert.All(competidores, c => Assert.Equal(categoria.Id, c.CategoriaId));
        await comp.Received(1).UpdateRangeAsync(Arg.Any<IEnumerable<Competidor>>(), Arg.Any<CancellationToken>());
        // Se generaron y persistieron las llaves, y la categoría quedó marcada.
        await llave.Received(1).AddRangeAsync(Arg.Any<IEnumerable<LlaveCompetencia>>(), Arg.Any<CancellationToken>());
        Assert.True(categoria.LlavesGeneradas);
        Assert.Equal(0, result.CompetidoresSinClasificar);
        Assert.True(result.Categorias[0].LlavesGeneradas);
    }

    [Fact]
    public async Task Handle_UnSoloCompetidor_NoGeneraLlaves()
    {
        var torneoId = Guid.NewGuid();
        var categoria = Categoria(torneoId);
        var competidores = new List<Competidor> { Competidor(torneoId) };
        var (tor, cat, comp, llave) = Repos(torneoId, [categoria], competidores);
        var handler = new GenerarLlavesCommandHandler(tor, cat, comp, llave);

        var result = await handler.Handle(new GenerarLlavesCommand(torneoId), CancellationToken.None);

        await llave.DidNotReceive().AddRangeAsync(Arg.Any<IEnumerable<LlaveCompetencia>>(), Arg.Any<CancellationToken>());
        Assert.False(categoria.LlavesGeneradas);
        Assert.False(result.Categorias[0].LlavesGeneradas);
    }

    [Fact]
    public async Task Handle_CompetidorQueNoEncaja_QuedaSinClasificar()
    {
        var torneoId = Guid.NewGuid();
        var categoria = Categoria(torneoId); // exige sexo M
        // Competidora F: no encaja en la categoría masculina.
        var competidores = new List<Competidor> { Competidor(torneoId, sexo: Sexo.F) };
        var (tor, cat, comp, llave) = Repos(torneoId, [categoria], competidores);
        var handler = new GenerarLlavesCommandHandler(tor, cat, comp, llave);

        var result = await handler.Handle(new GenerarLlavesCommand(torneoId), CancellationToken.None);

        Assert.Null(competidores[0].CategoriaId);
        Assert.Equal(1, result.CompetidoresSinClasificar);
    }

    [Fact]
    public async Task Handle_CategoriaConLlavesPrevias_NoRegenera()
    {
        var torneoId = Guid.NewGuid();
        var categoria = Categoria(torneoId);
        categoria.LlavesGeneradas = true;
        var competidores = new List<Competidor>
        {
            new() { Id = Guid.NewGuid(), TorneoId = torneoId, CategoriaId = categoria.Id, Sexo = Sexo.M, Edad = 13, Graduacion = "CinturonAmarillo" },
            new() { Id = Guid.NewGuid(), TorneoId = torneoId, CategoriaId = categoria.Id, Sexo = Sexo.M, Edad = 13, Graduacion = "CinturonAmarillo" },
        };
        var (tor, cat, comp, llave) = Repos(torneoId, [categoria], competidores);
        var handler = new GenerarLlavesCommandHandler(tor, cat, comp, llave);

        await handler.Handle(new GenerarLlavesCommand(torneoId), CancellationToken.None);

        await llave.DidNotReceive().AddRangeAsync(Arg.Any<IEnumerable<LlaveCompetencia>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Torneo?)null);
        var handler = new GenerarLlavesCommandHandler(tor, Substitute.For<ICategoriaRepository>(),
            Substitute.For<ICompetidorRepository>(), Substitute.For<ILlaveCompetenciaRepository>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GenerarLlavesCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
