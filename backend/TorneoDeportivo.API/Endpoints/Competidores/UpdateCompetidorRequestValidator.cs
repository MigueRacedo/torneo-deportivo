using FastEndpoints;
using FluentValidation;
using TorneoDeportivo.Application.Features.Competidores;

namespace TorneoDeportivo.API.Endpoints.Competidores;

/// <summary>
/// Valida el <see cref="UpdateCompetidorRequest"/> aplicando las reglas comunes de competidor
/// (datos personales, sexo, edad, graduación, peso y altura).
/// </summary>
public class UpdateCompetidorRequestValidator : Validator<UpdateCompetidorRequest>
{
    public UpdateCompetidorRequestValidator()
    {
        this.AddCompetidorRules();
    }
}
