using FastEndpoints;
using FluentValidation;

namespace TorneoDeportivo.API.Endpoints.Llaves;

/// <summary>
/// Valida el <see cref="RegistrarGanadorRequest"/>: el ganador debe estar presente.
/// </summary>
public class RegistrarGanadorRequestValidator : Validator<RegistrarGanadorRequest>
{
    public RegistrarGanadorRequestValidator()
    {
        RuleFor(x => x.GanadorId).NotEmpty().WithMessage("Elegí el ganador del match.");
    }
}
