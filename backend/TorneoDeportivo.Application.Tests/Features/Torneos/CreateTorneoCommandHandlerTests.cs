using NSubstitute;
using TorneoDeportivo.Application.Features.Torneos.Commands.CreateTorneo;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Interfaces;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Torneos;

public class CreateTorneoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsTorneoResponse()
    {
        var repo = Substitute.For<ITorneoRepository>();
        var handler = new CreateTorneoCommandHandler(repo);
        var command = new CreateTorneoCommand(
            "Copa Apertura", DateOnly.FromDateTime(DateTime.Today.AddDays(30)), "Club San Martín", null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(command.Nombre, result.Nombre);
        Assert.Equal(command.Lugar, result.Lugar);
        Assert.Equal(command.Fecha, result.Fecha);
        Assert.Equal("Borrador", result.Estado);
        await repo.Received(1).AddAsync(Arg.Is<Torneo>(t => t.Nombre == command.Nombre), Arg.Any<CancellationToken>());
    }
}
