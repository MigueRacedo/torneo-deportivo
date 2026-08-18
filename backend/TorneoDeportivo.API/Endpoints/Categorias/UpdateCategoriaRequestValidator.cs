using FastEndpoints;
using FluentValidation;
using TorneoDeportivo.Application.Features.Categorias;

namespace TorneoDeportivo.API.Endpoints.Categorias;

/// <summary>
/// Valida el <see cref="UpdateCategoriaRequest"/> aplicando las reglas comunes de categoría
/// (nombre, enums, rangos abiertos con mín &lt; máx, graduaciones válidas).
/// </summary>
public class UpdateCategoriaRequestValidator : Validator<UpdateCategoriaRequest>
{
    public UpdateCategoriaRequestValidator()
    {
        this.AddCategoriaRules();
    }
}
