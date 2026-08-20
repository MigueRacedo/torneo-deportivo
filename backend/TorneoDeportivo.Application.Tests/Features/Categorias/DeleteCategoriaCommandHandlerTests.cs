using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Categorias.Commands.DeleteCategoria;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Categorias;

public class DeleteCategoriaCommandHandlerTests
{
    private static Categoria CategoriaExistente(Guid id, Guid torneoId) => new()
    {
        Id = id,
        TorneoId = torneoId,
        Nombre = "Cadetes A"
    };

    private static ITorneoRepository TorneoRepoCon(Guid torneoId, EstadoTorneo estado)
    {
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = torneoId, Estado = estado });
        return repo;
    }

    [Fact]
    public async Task Handle_CategoriaExistente_LaElimina()
    {
        var id = Guid.NewGuid();
        var torneoId = Guid.NewGuid();
        var cat = Substitute.For<ICategoriaRepository>();
        var categoria = CategoriaExistente(id, torneoId);
        cat.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(categoria);
        var handler = new DeleteCategoriaCommandHandler(cat, TorneoRepoCon(torneoId, EstadoTorneo.Borrador));

        await handler.Handle(new DeleteCategoriaCommand(id, torneoId), CancellationToken.None);

        await cat.Received(1).DeleteAsync(categoria, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CategoriaInexistente_LanzaNotFoundException()
    {
        var cat = Substitute.For<ICategoriaRepository>();
        cat.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Categoria?)null);
        var handler = new DeleteCategoriaCommandHandler(cat, Substitute.For<ITorneoRepository>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new DeleteCategoriaCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
        await cat.DidNotReceive().DeleteAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CategoriaDeOtroTorneo_LanzaNotFoundException()
    {
        var id = Guid.NewGuid();
        var cat = Substitute.For<ICategoriaRepository>();
        cat.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CategoriaExistente(id, Guid.NewGuid()));
        var handler = new DeleteCategoriaCommandHandler(cat, Substitute.For<ITorneoRepository>());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new DeleteCategoriaCommand(id, Guid.NewGuid()), CancellationToken.None));
        await cat.DidNotReceive().DeleteAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CategoriaConLlavesGeneradas_LanzaConflictException()
    {
        var id = Guid.NewGuid();
        var torneoId = Guid.NewGuid();
        var cat = Substitute.For<ICategoriaRepository>();
        var categoria = CategoriaExistente(id, torneoId);
        categoria.LlavesGeneradas = true;
        cat.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(categoria);
        var handler = new DeleteCategoriaCommandHandler(cat, TorneoRepoCon(torneoId, EstadoTorneo.Borrador));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new DeleteCategoriaCommand(id, torneoId), CancellationToken.None));
        await cat.DidNotReceive().DeleteAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoFinalizado_LanzaConflictException()
    {
        var id = Guid.NewGuid();
        var torneoId = Guid.NewGuid();
        var cat = Substitute.For<ICategoriaRepository>();
        cat.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(CategoriaExistente(id, torneoId));
        var handler = new DeleteCategoriaCommandHandler(cat, TorneoRepoCon(torneoId, EstadoTorneo.Finalizado));

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(new DeleteCategoriaCommand(id, torneoId), CancellationToken.None));
        await cat.DidNotReceive().DeleteAsync(Arg.Any<Categoria>(), Arg.Any<CancellationToken>());
    }
}
