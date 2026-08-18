using FastEndpoints;
using FluentValidation;

namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Valida el <see cref="UpdateTorneoRequest"/>: nombre y lugar requeridos y fecha futura válida.
/// </summary>
public class UpdateTorneoRequestValidator : Validator<UpdateTorneoRequest>
{
    public UpdateTorneoRequestValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Fecha).GreaterThan(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.Lugar).NotEmpty().MaximumLength(300);
    }
}
