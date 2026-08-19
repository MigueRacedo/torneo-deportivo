using FluentValidation;

namespace TorneoDeportivo.Application.Features.Competidores.Commands.CreateCompetidor;

/// <summary>
/// Valida el <see cref="CreateCompetidorCommand"/>: ids presentes más las reglas comunes de competidor
/// (datos personales, edad, graduación, peso y altura).
/// </summary>
public class CreateCompetidorCommandValidator : AbstractValidator<CreateCompetidorCommand>
{
    public CreateCompetidorCommandValidator()
    {
        RuleFor(x => x.TorneoId).NotEmpty();
        this.AddCompetidorRules();
    }
}
