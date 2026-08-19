using FluentValidation;

namespace TorneoDeportivo.Application.Features.Competidores.Commands.UpdateCompetidor;

/// <summary>
/// Valida el <see cref="UpdateCompetidorCommand"/>: ids presentes más las reglas comunes de competidor
/// (datos personales, sexo, edad, graduación, peso y altura).
/// </summary>
public class UpdateCompetidorCommandValidator : AbstractValidator<UpdateCompetidorCommand>
{
    public UpdateCompetidorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TorneoId).NotEmpty();
        this.AddCompetidorRules();
    }
}
