using FastEndpoints;
using FluentValidation;
using TorneoDeportivo.Domain.Enums;

namespace TorneoDeportivo.API.Endpoints.Torneos;

/// <summary>
/// Valida el <see cref="CambiarEstadoTorneoRequest"/>: el estado destino debe ser un valor conocido.
/// </summary>
public class CambiarEstadoTorneoRequestValidator : Validator<CambiarEstadoTorneoRequest>
{
    public CambiarEstadoTorneoRequestValidator()
    {
        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("Indicá el estado al que querés mover el torneo.")
            .Must(v => Enum.TryParse<EstadoTorneo>(v, out _))
            .WithMessage("El estado debe ser Borrador, Activo o Finalizado.");
    }
}
