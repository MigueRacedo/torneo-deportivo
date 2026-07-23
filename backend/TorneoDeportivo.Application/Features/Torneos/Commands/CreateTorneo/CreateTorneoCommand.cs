using MediatR;

namespace TorneoDeportivo.Application.Features.Torneos.Commands.CreateTorneo;

/// <summary>
/// Comando CQRS que solicita la creación de un nuevo torneo (H0001).
/// </summary>
public record CreateTorneoCommand(string Nombre, DateOnly Fecha, string Lugar, string? ImagenFlyer)
    : IRequest<TorneoResponse>;
