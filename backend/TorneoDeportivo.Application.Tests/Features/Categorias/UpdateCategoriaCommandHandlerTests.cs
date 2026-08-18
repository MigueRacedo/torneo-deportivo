using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Categorias.Commands.UpdateCategoria;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Categorias;

public class UpdateCategoriaCommandHandlerTests
{
    private static UpdateCategoriaCommand Comando(Guid id, Guid torneoId, string tipo = "Combate") => new(
        id, torneoId, "Adultos B", tipo, "M",
        20, 40, 60m, 70m,
        "CinturonAzul", "CinturonRojo");

    private static Categoria CategoriaExistente(Guid id, Guid torneoId) => new()
    {
        Id = id,
        TorneoId = torneoId,
        Nombre = "Adultos A",
        TipoCompetencia = TipoCompetencia.Combate,
        Sexo = Sexo.F,
        RangoGraduacionMin = "CinturonAmarillo",
        RangoGraduacionMax = "CinturonVerde"
    };

    private static ITorneoRepository TorneoRepoCon(Guid torneoId, EstadoTorneo estado)
    {
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = torneoId, Estado = estado });
        return repo;
    }

    [Fact]
    public async Task Handle_CategoriaExistente_ActualizaYRetornaResponse()
    {
        var id = Guid.NewGuid();
        var torneoId = Guid.NewGuid();
        var repo = Substitute.For<ICategoriaRepository>();
        repo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CategoriaExistente(id, torneoId));
        var handler = new UpdateCategoriaCommandHandler(repo, TorneoRepoCon(torneoId, EstadoTorneo.Borrador));

        var result = await handler.Handle(Comando(id, torneoId), CancellationToken.None);

        Assert.Equal("Adultos B", result.Nombre);
        Assert.Equal("M", result.Sexo);
        Assert.Equal(60m, result.RangoPesoMin);
        await repo.Received(1).UpdateAsync(
            Arg.Is<Categoria>(c => c.Id == id && c.Nombre == "Adultos B" && c.Sexo == Sexo.M),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TipoFormas_DescartaElPeso()
    {
        var id = Guid.NewGuid();
        var torneoId = Guid.NewGuid();
        var repo = Substitute.For<ICategoriaRepository>();
        repo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CategoriaExistente(id, torneoId));
        var handler = new UpdateCategoriaCommandHandler(repo, TorneoRepoCon(torneoId, EstadoTorneo.Borrador));

        var result = await handler.Handle(Comando(id, torneoId, tipo: "Formas"), CancellationToken.None);

        Assert.Null(result.RangoPesoMin);
        Assert.Null(result.RangoPesoMax);
    }

    [Fact]
    public async Task Handle_CategoriaInexistente_LanzaNotFoundException()
    {
        var repo = Substitute.For<ICategoriaRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Categoria?)null);
        var handler = new UpdateCategoriaCommandHandler(repo, Substitute.For<ITorneoRepository>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(Comando(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CategoriaDeOtroTorneo_LanzaNotFoundException()
    {
        var id = Guid.NewGuid();
        var repo = Substitute.For<ICategoriaRepository>();
        // La categoría existe pero pertenece a otro torneo distinto al de la ruta.
        repo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CategoriaExistente(id, Guid.NewGuid()));
        var handler = new UpdateCategoriaCommandHandler(repo, Substitute.For<ITorneoRepository>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(Comando(id, Guid.NewGuid()), CancellationToken.None));
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoFinalizado_LanzaConflictException()
    {
        var id = Guid.NewGuid();
        var torneoId = Guid.NewGuid();
        var repo = Substitute.For<ICategoriaRepository>();
        repo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CategoriaExistente(id, torneoId));
        var handler = new UpdateCategoriaCommandHandler(repo, TorneoRepoCon(torneoId, EstadoTorneo.Finalizado));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(Comando(id, torneoId), CancellationToken.None));
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }
}
