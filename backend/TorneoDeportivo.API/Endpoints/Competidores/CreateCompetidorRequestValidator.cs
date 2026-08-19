using FastEndpoints;
using FluentValidation;
using TorneoDeportivo.Application.Features.Competidores;

namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Valida el <see cref="CreateCompetidorRequest"/> aplicando las reglas comunes de competidor
/// (datos personales, sexo, edad, graduación, peso y altura).
/// </summary>
public class CreateCompetidorRequestValidator : Validator<CreateCompetidorRequest>
{
    public CreateCompetidorRequestValidator()
    {
        this.AddCompetidorRules();
    }
}
