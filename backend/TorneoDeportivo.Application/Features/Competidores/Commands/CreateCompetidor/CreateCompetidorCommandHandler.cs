using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Competidores.Commands.CreateCompetidor;

/// <summary>
/// Handler CQRS que carga un competidor: valida que el torneo exista y no esté finalizado, que la categoría
/// exista y pertenezca a ese torneo, persiste el competidor y devuelve su representación.
/// </summary>
public class CreateCompetidorCommandHandler(
    ICompetidorRepository competidorRepo,
    ICategoriaRepository categoriaRepo,
    ITorneoRepository torneoRepo) : IRequestHandler<CreateCompetidorCommand, CompetidorResponse>
{
    /// <summary>
    /// Verifica torneo (existencia y estado) y categoría (existencia y pertenencia al torneo), arma la entidad
    /// <see cref="Competidor"/>, la guarda y devuelve la respuesta con el nombre de la categoría.
    /// </summary>
    public async Task<CompetidorResponse> Handle(CreateCompetidorCommand request, CancellationToken ct)
    {
        var torneo = await torneoRepo.GetByIdAsync(request.TorneoId, ct)
            ?? throw new NotFoundException(nameof(Torneo), request.TorneoId);

        if (torneo.Estado == EstadoTorneo.Finalizado)
            throw new ConflictException("No se pueden cargar competidores en un torneo finalizado.");

        var categoria = await categoriaRepo.GetByIdAsync(request.CategoriaId, ct);
        if (categoria is null || categoria.TorneoId != request.TorneoId)
            throw new NotFoundException(nameof(Categoria), request.CategoriaId);

        var competidor = new Competidor
        {
            Id = Guid.NewGuid(),
            TorneoId = torneo.Id,
            CategoriaId = categoria.Id,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Edad = request.Edad,
            Graduacion = request.Graduacion,
            Peso = request.Peso,
            Altura = request.Altura,
            Escuela = request.Escuela,
            Responsable = request.Responsable,
            Telefono = request.Telefono
        };

        await competidorRepo.AddAsync(competidor, ct);

        return competidor.ToResponse(categoria.Nombre);
    }
}
