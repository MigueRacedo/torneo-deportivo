using FluentValidation;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.CreateTorneo;

public class CreateTorneoCommandValidator : AbstractValidator<CreateTorneoCommand>
{
    public CreateTorneoCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Fecha).GreaterThan(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.Lugar).NotEmpty().MaximumLength(300);
    }
}
