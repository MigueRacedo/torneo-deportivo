using FluentValidation;

namespace TorneoDeportivo.Application.Features.Categorias.Commands.CreateCategoria;

/// <summary>
/// Valida el <see cref="CreateCategoriaCommand"/> aplicando las reglas comunes de categoría
/// (nombre, enums, rangos abiertos con mín &lt; máx, graduaciones válidas).
/// </summary>
public class CreateCategoriaCommandValidator : AbstractValidator<CreateCategoriaCommand>
{
    public CreateCategoriaCommandValidator()
    {
        this.AddCategoriaRules();
    }
}
