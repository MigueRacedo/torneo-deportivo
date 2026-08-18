using TorneoDeportivo.Application.Features.Torneos.Commands.UpdateTorneo;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Torneos;

public class UpdateTorneoCommandValidatorTests
{
    private readonly UpdateTorneoCommandValidator _validator = new();

    private static UpdateTorneoCommand Base(
        Guid? id = null, string nombre = "Copa Clausura", int diasFecha = 20, string lugar = "Club Nuevo") => new(
        id ?? Guid.NewGuid(), nombre, DateOnly.FromDateTime(DateTime.Today.AddDays(diasFecha)), lugar, null);

    [Fact]
    public void Validate_ComandoValido_NoTieneErrores()
    {
        Assert.True(_validator.Validate(Base()).IsValid);
    }

    [Fact]
    public void Validate_IdVacio_FallaConError()
    {
        var result = _validator.Validate(Base(id: Guid.Empty));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateTorneoCommand.Id));
    }

    [Fact]
    public void Validate_NombreVacio_FallaConError()
    {
        var result = _validator.Validate(Base(nombre: ""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateTorneoCommand.Nombre));
    }

    [Fact]
    public void Validate_FechaPasada_FallaConError()
    {
        var result = _validator.Validate(Base(diasFecha: -1));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateTorneoCommand.Fecha));
    }
}
