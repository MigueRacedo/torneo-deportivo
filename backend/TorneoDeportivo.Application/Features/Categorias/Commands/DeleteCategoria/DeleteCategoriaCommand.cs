using MediatR;

namespace TorneoDeportivo.Application.Features.Categorias.Commands.DeleteCategoria;

/// <summary>
/// Comando CQRS que solicita la eliminación (física) de una categoría de un torneo.
/// </summary>
public record DeleteCategoriaCommand(Guid Id, Guid TorneoId) : IRequest;
