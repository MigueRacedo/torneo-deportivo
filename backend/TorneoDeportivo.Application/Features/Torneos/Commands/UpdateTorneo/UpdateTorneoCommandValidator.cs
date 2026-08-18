using FluentValidation;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.UpdateTorneo;

/// <summary>
/// Valida el <see cref="UpdateTorneoCommand"/>: id presente, nombre y lugar requeridos y fecha futura válida.
/// </summary>
public class UpdateTorneoCommandValidator : AbstractValidator<UpdateTorneoCommand>
{
    public UpdateTorneoCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Fecha).GreaterThan(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.Lugar).NotEmpty().MaximumLength(300);
    }
}
