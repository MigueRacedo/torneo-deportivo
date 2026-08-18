using FluentValidation;
using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Application.Features.Competidores;

/// <summary>
/// Reglas de validación compartidas para los datos de un competidor, reutilizadas por el validator del comando
/// (Application) y del request (API) sobre <see cref="ICompetidorData"/>.
/// </summary>
public static class CompetidorValidationRules
{
    /// <summary>
    /// Agrega al validador las reglas de datos personales, edad (1–120), graduación válida, peso y altura
    /// dentro de rangos físicos razonables, y escuela/responsable requeridos.
    /// </summary>
    public static void AddCompetidorRules<T>(this AbstractValidator<T> v) where T : ICompetidorData
    {
        v.RuleFor(x => x.Nombre).NotEmpty().MaximumLength(150);
        v.RuleFor(x => x.Apellido).NotEmpty().MaximumLength(150);

        v.RuleFor(x => x.Edad)
            .InclusiveBetween(1, 120)
            .WithMessage("La edad debe estar entre 1 y 120 años.");

        v.RuleFor(x => x.Graduacion)
            .Must(val => Enum.TryParse<Graduacion>(val, out _))
            .WithMessage("La graduación no es una graduación válida.");

        v.RuleFor(x => x.Peso)
            .GreaterThan(0).LessThanOrEqualTo(500m)
            .WithMessage("El peso debe ser mayor a 0 y hasta 500 kg.");

        v.RuleFor(x => x.Altura)
            .GreaterThan(0).LessThanOrEqualTo(3m)
            .WithMessage("La altura debe ser mayor a 0 y hasta 3 metros.");

        v.RuleFor(x => x.Escuela).NotEmpty().MaximumLength(200);
        v.RuleFor(x => x.Responsable).NotEmpty().MaximumLength(200);
        v.RuleFor(x => x.Telefono).MaximumLength(50);
    }
}
