using FluentValidation;

namespace TorneoDeportivo.Application.Features.Categorias.Commands.UpdateCategoria;

/// <summary>
/// Valida el <see cref="UpdateCategoriaCommand"/>: ids presentes más las reglas comunes de categoría
/// (nombre, enums, rangos abiertos con mín &lt; máx, graduaciones válidas).
/// </summary>
public class UpdateCategoriaCommandValidator : AbstractValidator<UpdateCategoriaCommand>
{
    public UpdateCategoriaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TorneoId).NotEmpty();
        this.AddCategoriaRules();
    }
}
