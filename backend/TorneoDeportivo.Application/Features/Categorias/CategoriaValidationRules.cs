using FluentValidation;
using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Application.Features.Categorias;

/// <summary>
/// Reglas de validación compartidas para los datos de una categoría (alta y edición),
/// reutilizadas por los validators de comandos (Application) y de requests (API) sobre <see cref="ICategoriaData"/>.
/// </summary>
public static class CategoriaValidationRules
{
    /// <summary>
    /// Agrega al validador las reglas de nombre, enums (TipoCompetencia/Sexo), rangos de edad y peso
    /// opcionales y abiertos (mínimo estrictamente menor al máximo cuando ambos existen) y graduaciones
    /// válidas (la mínima puede igualar a la máxima).
    /// </summary>
    public static void AddCategoriaRules<T>(this AbstractValidator<T> v) where T : ICategoriaData
    {
        v.RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);

        v.RuleFor(x => x.TipoCompetencia)
            .Must(val => Enum.TryParse<TipoCompetencia>(val, out _))
            .WithMessage("El tipo de competencia debe ser 'Combate' o 'Formas'.");

        v.RuleFor(x => x.Sexo)
            .Must(val => Enum.TryParse<Sexo>(val, out _))
            .WithMessage("El sexo debe ser 'M' o 'F'.");

        // Edad: cada extremo es opcional (rango abierto). Si ambos existen, mín < máx (estricto).
        v.RuleFor(x => x.RangoEdadMin!.Value).GreaterThanOrEqualTo(0).When(x => x.RangoEdadMin.HasValue);
        v.RuleFor(x => x.RangoEdadMax!.Value).GreaterThanOrEqualTo(0).When(x => x.RangoEdadMax.HasValue);
        v.RuleFor(x => x.RangoEdadMax)
            .Must((x, max) => max > x.RangoEdadMin)
            .When(x => x.RangoEdadMin.HasValue && x.RangoEdadMax.HasValue)
            .WithName(nameof(ICategoriaData.RangoEdadMax))
            .WithMessage("La edad máxima debe ser mayor que la mínima (no pueden ser iguales).");

        // Peso: opcional (no aplica a Formas). Si ambos existen, mín < máx (estricto).
        v.RuleFor(x => x.RangoPesoMin!.Value).GreaterThan(0).When(x => x.RangoPesoMin.HasValue);
        v.RuleFor(x => x.RangoPesoMax!.Value).GreaterThan(0).When(x => x.RangoPesoMax.HasValue);
        v.RuleFor(x => x.RangoPesoMax)
            .Must((x, max) => max > x.RangoPesoMin)
            .When(x => x.RangoPesoMin.HasValue && x.RangoPesoMax.HasValue)
            .WithName(nameof(ICategoriaData.RangoPesoMax))
            .WithMessage("El peso máximo debe ser mayor que el mínimo (no pueden ser iguales).");

        // Graduación: obligatoria y válida; la mínima puede igualar a la máxima.
        v.RuleFor(x => x.RangoGraduacionMin)
            .Must(val => Enum.TryParse<Graduacion>(val, out _))
            .WithMessage("La graduación mínima no es una graduación válida.");
        v.RuleFor(x => x.RangoGraduacionMax)
            .Must(val => Enum.TryParse<Graduacion>(val, out _))
            .WithMessage("La graduación máxima no es una graduación válida.");
        v.RuleFor(x => x)
            .Must(x => Enum.TryParse<Graduacion>(x.RangoGraduacionMin, out var min)
                    && Enum.TryParse<Graduacion>(x.RangoGraduacionMax, out var max)
                    && max >= min)
            .When(x => Enum.TryParse<Graduacion>(x.RangoGraduacionMin, out _)
                    && Enum.TryParse<Graduacion>(x.RangoGraduacionMax, out _))
            .WithName(nameof(ICategoriaData.RangoGraduacionMax))
            .WithMessage("La graduación máxima no puede ser menor que la mínima.");
    }
}
