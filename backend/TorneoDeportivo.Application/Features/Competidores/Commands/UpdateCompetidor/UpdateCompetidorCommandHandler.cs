using MediatR;
using TorneoDeportivo.Application.Common.Exceptions;
using TorneoDeportivo.Domain.Entities;
using TorneoDeportivo.Domain.Enums;
using TorneoDeportivo.Domain.Interfaces;

namespace TorneoDeportivo.Application.Features.Competidores.Commands.UpdateCompetidor;

/// <summary>
/// Handler CQRS que edita un competidor: valida que exista y pertenezca al torneo indicado, impide editar
/// competidores de un torneo finalizado, aplica los nuevos datos y devuelve la representación actualizada.
/// </summary>
public class UpdateCompetidorCommandHandler(
    ICompetidorRepository competidorRepo,
    ITorneoRepository torneoRepo) : IRequestHandler<UpdateCompetidorCommand, CompetidorResponse>
{
    /// <summary>
    /// Busca el competidor (404 si no existe o no pertenece al torneo de la ruta), rechaza la edición si el torneo
    /// está Finalizado (409), aplica los cambios (sin tocar la categoría) y devuelve la respuesta.
    /// </summary>
    public async Task<CompetidorResponse> Handle(UpdateCompetidorCommand request, CancellationToken ct)
    {
        var competidor = await competidorRepo.GetByIdAsync(request.Id, ct);
        if (competidor is null || competidor.TorneoId != request.TorneoId)
            throw new NotFoundException(nameof(Competidor), request.Id);

        var torneo = await torneoRepo.GetByIdAsync(competidor.TorneoId, ct);
        if (torneo is null || torneo.Estado != EstadoTorneo.Borrador)
            throw new ConflictException("No se pueden editar los competidores de un torneo que ya arrancó. Solo se puede en Borrador.");

        competidor.Nombre = request.Nombre;
        competidor.Apellido = request.Apellido;
        competidor.Sexo = Enum.Parse<Sexo>(request.Sexo);
        competidor.Edad = request.Edad;
        competidor.Graduacion = request.Graduacion;
        competidor.Peso = request.Peso;
        competidor.Altura = request.Altura;
        competidor.Escuela = request.Escuela;
        competidor.Responsable = request.Responsable;
        competidor.Telefono = request.Telefono;

        await competidorRepo.UpdateAsync(competidor, ct);

        return competidor.ToResponse();
    }
}
