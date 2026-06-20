using Mapster;
using MediatR;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.CreateTorneo;

public class CreateTorneoCommandHandler(ITorneoRepository repo) : IRequestHandler<CreateTorneoCommand, TorneoResponse>
{
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
