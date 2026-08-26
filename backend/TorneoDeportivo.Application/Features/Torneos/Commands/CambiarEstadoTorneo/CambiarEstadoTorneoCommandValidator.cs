using FluentValidation;
using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.CambiarEstadoTorneo;

/// <summary>
/// Valida el <see cref="CambiarEstadoTorneoCommand"/>: el estado destino debe ser uno de los valores
/// de <see cref="EstadoTorneo"/>.
/// </summary>
public class CambiarEstadoTorneoCommandValidator : AbstractValidator<CambiarEstadoTorneoCommand>
{
    public CambiarEstadoTorneoCommandValidator()
    {
        RuleFor(x => x.TorneoId).NotEmpty();
        RuleFor(x => x.NuevoEstado)
            .Must(v => Enum.TryParse<EstadoTorneo>(v, out _))
            .WithMessage("El estado debe ser Borrador, Activo o Finalizado.");
    }
}
