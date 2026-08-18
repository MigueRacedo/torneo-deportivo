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
    private static CreateCompetidorCommand Comando(Guid torneoId, Guid categoriaId) => new(
        torneoId, categoriaId, "Juan", "Pérez", 15, "CinturonVerde",
        45.5m, 1.65m, "Escuela Central", "María Pérez", "1122334455");

    private static (ICompetidorRepository comp, ICategoriaRepository cat, ITorneoRepository tor) Repos(
        Guid torneoId, Guid categoriaId, EstadoTorneo estado = EstadoTorneo.Borrador, bool categoriaExiste = true,
        Guid? categoriaTorneoId = null)
    {
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(torneoId, Arg.Any<CancellationToken>())
            .Returns(new Torneo { Id = torneoId, Estado = estado });

        var cat = Substitute.For<ICategoriaRepository>();
        cat.GetByIdAsync(categoriaId, Arg.Any<CancellationToken>()).Returns(
            categoriaExiste
                ? new Categoria { Id = categoriaId, TorneoId = categoriaTorneoId ?? torneoId, Nombre = "Cadetes A" }
                : null);

        return (Substitute.For<ICompetidorRepository>(), cat, tor);
    }

    [Fact]
    public async Task Handle_DatosValidos_CargaCompetidorYRetornaResponse()
    {
        var torneoId = Guid.NewGuid();
        var categoriaId = Guid.NewGuid();
        var (comp, cat, tor) = Repos(torneoId, categoriaId);
        var handler = new CreateCompetidorCommandHandler(comp, cat, tor);

        var result = await handler.Handle(Comando(torneoId, categoriaId), CancellationToken.None);

        Assert.Equal("Juan Pérez", result.NombreCompleto);
        Assert.Equal("Cadetes A", result.CategoriaNombre);
        Assert.Equal(categoriaId, result.CategoriaId);
        await comp.Received(1).AddAsync(
            Arg.Is<Competidor>(c => c.Nombre == "Juan" && c.CategoriaId == categoriaId && c.TorneoId == torneoId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var tor = Substitute.For<ITorneoRepository>();
        tor.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Torneo?)null);
        var comp = Substitute.For<ICompetidorRepository>();
        var handler = new CreateCompetidorCommandHandler(comp, Substitute.For<ICategoriaRepository>(), tor);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(Comando(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
        await comp.DidNotReceive().AddAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TorneoFinalizado_LanzaConflictException()
    {
        var torneoId = Guid.NewGuid();
        var categoriaId = Guid.NewGuid();
        var (comp, cat, tor) = Repos(torneoId, categoriaId, estado: EstadoTorneo.Finalizado);
        var handler = new CreateCompetidorCommandHandler(comp, cat, tor);

        await Assert.ThrowsAsync<ConflictException>(
            () => handler.Handle(Comando(torneoId, categoriaId), CancellationToken.None));
        await comp.DidNotReceive().AddAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CategoriaInexistente_LanzaNotFoundException()
    {
        var torneoId = Guid.NewGuid();
        var categoriaId = Guid.NewGuid();
        var (comp, cat, tor) = Repos(torneoId, categoriaId, categoriaExiste: false);
        var handler = new CreateCompetidorCommandHandler(comp, cat, tor);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(Comando(torneoId, categoriaId), CancellationToken.None));
        await comp.DidNotReceive().AddAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CategoriaDeOtroTorneo_LanzaNotFoundException()
    {
        var torneoId = Guid.NewGuid();
        var categoriaId = Guid.NewGuid();
        // La categoría existe pero pertenece a otro torneo distinto al de la ruta.
        var (comp, cat, tor) = Repos(torneoId, categoriaId, categoriaTorneoId: Guid.NewGuid());
        var handler = new CreateCompetidorCommandHandler(comp, cat, tor);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(Comando(torneoId, categoriaId), CancellationToken.None));
        await comp.DidNotReceive().AddAsync(Arg.Any<Competidor>(), Arg.Any<CancellationToken>());
    }
}
