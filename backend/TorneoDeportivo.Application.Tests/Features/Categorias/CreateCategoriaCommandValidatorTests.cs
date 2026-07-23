using TorneoDeportivo.Application.Features.Categorias.Commands.CreateCategoria;
using Xunit;

namespace TorneoDeportivo.Application.Tests.Features.Categorias;

public class CreateCategoriaCommandValidatorTests
{
    private readonly CreateCategoriaCommandValidator _validator = new();

    private static CreateCategoriaCommand Base(
        string tipo = "Combate", string sexo = "F",
        int edadMin = 18, int edadMax = 35,
        decimal pesoMin = 50m, decimal pesoMax = 55m,
        string nombre = "Adultos A",
        string gradMin = "CinturonAmarillo", string gradMax = "CinturonVerde") => new(
        Guid.NewGuid(), nombre, tipo, sexo,
        edadMin, edadMax, pesoMin, pesoMax,
        gradMin, gradMax);

    [Fact]
    public void Validate_ComandoValido_NoTieneErrores()
    {
        var result = _validator.Validate(Base());
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_TipoCompetenciaInvalido_FallaConError()
    {
        var result = _validator.Validate(Base(tipo: "Boxeo"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCategoriaCommand.TipoCompetencia));
    }

    [Fact]
    public void Validate_SexoInvalido_FallaConError()
    {
        var result = _validator.Validate(Base(sexo: "X"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCategoriaCommand.Sexo));
    }

    [Fact]
    public void Validate_EdadMaxMenorQueMin_FallaConError()
    {
        var result = _validator.Validate(Base(edadMin: 30, edadMax: 20));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCategoriaCommand.RangoEdadMax));
    }

    [Fact]
    public void Validate_PesoMaxMenorQueMin_FallaConError()
    {
        var result = _validator.Validate(Base(pesoMin: 60m, pesoMax: 55m));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCategoriaCommand.RangoPesoMax));
    }

    [Fact]
    public void Validate_NombreVacio_FallaConError()
    {
        var result = _validator.Validate(Base(nombre: ""));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCategoriaCommand.Nombre));
    }

    [Fact]
    public void Validate_GraduacionInvalida_FallaConError()
    {
        var result = _validator.Validate(Base(gradMin: "CinturonArcoiris"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCategoriaCommand.RangoGraduacionMin));
    }

    [Fact]
    public void Validate_GraduacionMaxMenorQueMin_FallaConError()
    {
        var result = _validator.Validate(Base(gradMin: "CinturonVerde", gradMax: "CinturonAmarillo"));
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCategoriaCommand.RangoGraduacionMax));
    }
}
