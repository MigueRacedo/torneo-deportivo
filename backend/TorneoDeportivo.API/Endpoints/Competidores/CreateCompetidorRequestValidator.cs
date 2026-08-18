using FastEndpoints;
using FluentValidation;
using TorneoDeportivo.Application.Features.Competidores;

namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Valida el <see cref="CreateCompetidorRequest"/>: categoría presente más las reglas comunes de competidor
/// (datos personales, edad, graduación, peso y altura).
/// </summary>
public class CreateCompetidorRequestValidator : Validator<CreateCompetidorRequest>
{
    public CreateCompetidorRequestValidator()
    {
        RuleFor(x => x.CategoriaId).NotEmpty().WithMessage("Elegí una categoría para el competidor.");
        this.AddCompetidorRules();
    }
}
