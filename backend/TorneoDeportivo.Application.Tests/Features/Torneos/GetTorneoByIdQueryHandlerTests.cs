using NSubstitute;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Application.Features.Torneos.Queries.GetTorneoById;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Torneos;

public class GetTorneoByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_TorneoExistente_ReturnsTorneoResponse()
    {
        var torneoId = Guid.NewGuid();
        var torneo = new Torneo
        {
            Id = torneoId,
            Nombre = "Copa Apertura",
            Fecha = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            Lugar = "Club San Martín",
            Estado = EstadoTorneo.Borrador
        };
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(torneoId, Arg.Any<CancellationToken>()).Returns(torneo);
        var handler = new GetTorneoByIdQueryHandler(repo);

        var result = await handler.Handle(new GetTorneoByIdQuery(torneoId), CancellationToken.None);

        Assert.Equal(torneoId, result.Id);
        Assert.Equal("Copa Apertura", result.Nombre);
    }

    [Fact]
    public async Task Handle_TorneoInexistente_LanzaNotFoundException()
    {
        var repo = Substitute.For<ITorneoRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Torneo?)null);
        var handler = new GetTorneoByIdQueryHandler(repo);

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.Handle(new GetTorneoByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
