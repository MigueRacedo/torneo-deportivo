using MediatR;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.CreateTorneo;

public record CreateTorneoCommand(string Nombre, DateOnly Fecha, string Lugar, string? ImagenFlyer)
    : IRequest<TorneoResponse>;
