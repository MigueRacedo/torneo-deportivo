using FastEndpoints;
using FluentValidation;
using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.API.Endpoints.Categorias;

/// <summary>
/// Valida el <see cref="CreateCategoriaRequest"/>: nombre, enums válidos (TipoCompetencia y Sexo),
/// rangos de edad y peso opcionales y abiertos (mínimo estrictamente menor al máximo cuando ambos existen)
/// y graduaciones válidas (la mínima puede igualar a la máxima).
/// </summary>
public class CreateCategoriaRequestValidator : Validator<CreateCategoriaRequest>
{
    public CreateCategoriaRequestValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);

        RuleFor(x => x.TipoCompetencia)
            .Must(v => Enum.TryParse<TipoCompetencia>(v, out _))
            .WithMessage("El tipo de competencia debe ser 'Combate' o 'Formas'.");

        RuleFor(x => x.Sexo)
            .Must(v => Enum.TryParse<Sexo>(v, out _))
            .WithMessage("El sexo debe ser 'M' o 'F'.");

        // Edad: cada extremo es opcional (rango abierto). Si ambos existen, mín < máx (estricto).
        RuleFor(x => x.RangoEdadMin!.Value).GreaterThanOrEqualTo(0).When(x => x.RangoEdadMin.HasValue);
        RuleFor(x => x.RangoEdadMax!.Value).GreaterThanOrEqualTo(0).When(x => x.RangoEdadMax.HasValue);
        RuleFor(x => x.RangoEdadMax)
            .Must((x, max) => max > x.RangoEdadMin)
            .When(x => x.RangoEdadMin.HasValue && x.RangoEdadMax.HasValue)
            .WithMessage("La edad máxima debe ser mayor que la mínima (no pueden ser iguales).");

        // Peso: opcional (no aplica a Formas). Si ambos existen, mín < máx (estricto).
        RuleFor(x => x.RangoPesoMin!.Value).GreaterThan(0).When(x => x.RangoPesoMin.HasValue);
        RuleFor(x => x.RangoPesoMax!.Value).GreaterThan(0).When(x => x.RangoPesoMax.HasValue);
        RuleFor(x => x.RangoPesoMax)
            .Must((x, max) => max > x.RangoPesoMin)
            .When(x => x.RangoPesoMin.HasValue && x.RangoPesoMax.HasValue)
            .WithMessage("El peso máximo debe ser mayor que el mínimo (no pueden ser iguales).");

        // Graduación: obligatoria y válida; la mínima puede igualar a la máxima.
        RuleFor(x => x.RangoGraduacionMin)
            .Must(v => Enum.TryParse<Graduacion>(v, out _))
            .WithMessage("La graduación mínima no es una graduación válida.");
        RuleFor(x => x.RangoGraduacionMax)
            .Must(v => Enum.TryParse<Graduacion>(v, out _))
            .WithMessage("La graduación máxima no es una graduación válida.");
        RuleFor(x => x)
            .Must(x => Enum.TryParse<Graduacion>(x.RangoGraduacionMin, out var min)
                    && Enum.TryParse<Graduacion>(x.RangoGraduacionMax, out var max)
                    && max >= min)
            .When(x => Enum.TryParse<Graduacion>(x.RangoGraduacionMin, out _)
                    && Enum.TryParse<Graduacion>(x.RangoGraduacionMax, out _))
            .WithName(nameof(CreateCategoriaRequest.RangoGraduacionMax))
            .WithMessage("La graduación máxima no puede ser menor que la mínima.");
    }
}
