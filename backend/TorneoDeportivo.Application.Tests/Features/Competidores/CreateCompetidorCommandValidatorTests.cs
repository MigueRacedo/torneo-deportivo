using TorneoDeportivo.Application.Features.Competidores.Commands.CreateCompetidor;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Competidores;

public class CreateCompetidorCommandValidatorTests
{
    private readonly CreateCompetidorCommandValidator _validator = new();

    private static CreateCompetidorCommand Base(
        string nombre = "Juan", int edad = 15, string graduacion = "CinturonVerde",
        decimal peso = 45.5m, decimal altura = 1.65m, Guid? categoriaId = null) => new(
        Guid.NewGuid(), categoriaId ?? Guid.NewGuid(), nombre, "Pérez", edad, graduacion,
        peso, altura, "Escuela Central", "María Pérez", null);

    [Fact]
    public void Validate_ComandoValido_NoTieneErrores()
    {
        Assert.True(_validator.Validate(Base()).IsValid);
    }

    [Fact]
    public void Validate_NombreVacio_FallaConError()
    {
        var result = _validator.Validate(Base(nombre: ""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCompetidorCommand.Nombre));
    }

    [Fact]
    public void Validate_CategoriaVacia_FallaConError()
    {
        var result = _validator.Validate(Base(categoriaId: Guid.Empty));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCompetidorCommand.CategoriaId));
    }

    [Fact]
    public void Validate_EdadFueraDeRango_FallaConError()
    {
        var result = _validator.Validate(Base(edad: 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCompetidorCommand.Edad));
    }

    [Fact]
    public void Validate_GraduacionInvalida_FallaConError()
    {
        var result = _validator.Validate(Base(graduacion: "CinturonArcoiris"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCompetidorCommand.Graduacion));
    }

    [Fact]
    public void Validate_PesoInvalido_FallaConError()
    {
        var result = _validator.Validate(Base(peso: 0));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCompetidorCommand.Peso));
    }

    [Fact]
    public void Validate_AlturaFueraDeRango_FallaConError()
    {
        var result = _validator.Validate(Base(altura: 5m));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCompetidorCommand.Altura));
    }
}
