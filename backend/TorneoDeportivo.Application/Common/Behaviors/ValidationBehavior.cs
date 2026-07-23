using FluentValidation;
using MediatR;

namespace TorneoDeportivo.Application.Common.Behaviors;

/// <summary>
/// Comportamiento de pipeline de MediatR que ejecuta todos los validadores de FluentValidation registrados
/// para el request antes de invocar el handler, lanzando <see cref="ValidationException"/> si hay errores.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Valida el request contra todos los validadores disponibles; si no hay errores continúa con el siguiente
    /// paso de la pipeline (típicamente el handler).
    /// </summary>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct))))
            .SelectMany(result => result.Errors)
            .ToList();

        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next();
    }
}
