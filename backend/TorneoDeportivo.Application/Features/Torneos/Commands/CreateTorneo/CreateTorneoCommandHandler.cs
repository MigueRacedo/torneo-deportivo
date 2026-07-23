using Mapster;
using MediatR;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.CreateTorneo;

/// <summary>
/// Handler CQRS que crea un torneo nuevo en estado Borrador y lo persiste mediante el repositorio.
/// </summary>
public class CreateTorneoCommandHandler(ITorneoRepository repo) : IRequestHandler<CreateTorneoCommand, TorneoResponse>
{
    /// <summary>
    /// Construye la entidad <see cref="Torneo"/> a partir del comando, la guarda y devuelve su representación
    /// de respuesta.
    /// </summary>
    public async Task<TorneoResponse> Handle(CreateTorneoCommand request, CancellationToken ct)
    {
        var torneo = new Torneo
        {
            Id = Guid.NewGuid(),
            Nombre = request.Nombre,
            Fecha = request.Fecha,
            Lugar = request.Lugar,
            ImagenFlyer = request.ImagenFlyer,
            Estado = EstadoTorneo.Borrador
        };

        await repo.AddAsync(torneo, ct);

        return torneo.Adapt<TorneoResponse>();
    }
}
