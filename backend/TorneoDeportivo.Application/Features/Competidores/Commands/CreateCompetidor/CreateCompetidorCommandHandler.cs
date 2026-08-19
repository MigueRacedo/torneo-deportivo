using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Competidores.Commands.CreateCompetidor;

/// <summary>
/// Handler CQRS que carga un competidor en un torneo: valida que el torneo exista y no esté finalizado,
/// persiste el competidor (sin categoría) y devuelve su representación.
/// </summary>
public class CreateCompetidorCommandHandler(
    ICompetidorRepository competidorRepo,
    ITorneoRepository torneoRepo) : IRequestHandler<CreateCompetidorCommand, CompetidorResponse>
{
    /// <summary>
    /// Verifica el torneo (existencia y estado), arma la entidad <see cref="Competidor"/> sin categoría,
    /// la guarda y devuelve la respuesta.
    /// </summary>
    public async Task<CompetidorResponse> Handle(CreateCompetidorCommand request, CancellationToken ct)
    {
        var torneo = await torneoRepo.GetByIdAsync(request.TorneoId, ct)
            ?? throw new NotFoundException(nameof(Torneo), request.TorneoId);

        if (torneo.Estado == EstadoTorneo.Finalizado)
            throw new ConflictException("No se pueden cargar competidores en un torneo finalizado.");

        var competidor = new Competidor
        {
            Id = Guid.NewGuid(),
            TorneoId = torneo.Id,
            CategoriaId = null,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Sexo = Enum.Parse<Sexo>(request.Sexo),
            Edad = request.Edad,
            Graduacion = request.Graduacion,
            Peso = request.Peso,
            Altura = request.Altura,
            Escuela = request.Escuela,
            Responsable = request.Responsable,
            Telefono = request.Telefono
        };

        await competidorRepo.AddAsync(competidor, ct);

        return competidor.ToResponse();
    }
}
