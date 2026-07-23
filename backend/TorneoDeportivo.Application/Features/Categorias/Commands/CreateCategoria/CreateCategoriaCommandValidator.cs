using FluentValidation;
using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Application.Features.Categorias.Commands.CreateCategoria;

/// <summary>
/// Valida el <see cref="CreateCategoriaCommand"/>: nombre, enums válidos (TipoCompetencia y Sexo),
/// rangos de edad y peso coherentes (mínimo menor o igual al máximo) y graduaciones presentes.
/// </summary>
public class CreateCategoriaCommandValidator : AbstractValidator<CreateCategoriaCommand>
{
    public CreateCategoriaCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);

        RuleFor(x => x.TipoCompetencia)
            .Must(v => Enum.TryParse<TipoCompetencia>(v, out _))
            .WithMessage("El tipo de competencia debe ser 'Combate' o 'Formas'.");

        RuleFor(x => x.Sexo)
            .Must(v => Enum.TryParse<Sexo>(v, out _))
            .WithMessage("El sexo debe ser 'M' o 'F'.");

        RuleFor(x => x.RangoEdadMin).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RangoEdadMax)
            .GreaterThanOrEqualTo(x => x.RangoEdadMin)
            .WithMessage("La edad máxima no puede ser menor que la edad mínima.");

        RuleFor(x => x.RangoPesoMin).GreaterThan(0);
        RuleFor(x => x.RangoPesoMax)
            .GreaterThanOrEqualTo(x => x.RangoPesoMin)
            .WithMessage("El peso máximo no puede ser menor que el peso mínimo.");

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
            .WithName(nameof(CreateCategoriaCommand.RangoGraduacionMax))
            .WithMessage("La graduación máxima no puede ser menor que la mínima.");
    }
}
