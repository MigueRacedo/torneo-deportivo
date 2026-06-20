using FastEndpoints;
using FluentValidation;

namespace TorneoDeportivo.API.Endpoints.Torneos;

public class CreateTorneoRequestValidator : Validator<CreateTorneoRequest>
{
    public CreateTorneoRequestValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Fecha).GreaterThan(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.Lugar).NotEmpty().MaximumLength(300);
    }
}
