using TorneoDeportivo.Application.Features.Torneos.Commands.CreateTorneo;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Torneos;

public class CreateTorneoCommandValidatorTests
{
    private readonly CreateTorneoCommandValidator _validator = new();

    [Fact]
    public void Validate_NombreVacio_FallaConError()
    {
        var command = new CreateTorneoCommand(
            "", DateOnly.FromDateTime(DateTime.Today.AddDays(10)), "Club San Martín", null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateTorneoCommand.Nombre));
    }

    [Fact]
    public void Validate_FechaPasada_FallaConError()
    {
        var command = new CreateTorneoCommand(
            "Copa Apertura", DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), "Club San Martín", null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateTorneoCommand.Fecha));
    }

    [Fact]
    public void Validate_ComandoValido_NoTieneErrores()
    {
        var command = new CreateTorneoCommand(
            "Copa Apertura", DateOnly.FromDateTime(DateTime.Today.AddDays(10)), "Club San Martín", null);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
