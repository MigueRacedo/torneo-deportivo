using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TorneoDeportivo.Application.Common.Behaviors;

namespace TorneoDeportivo.Application;

/// <summary>
/// Registro de servicios de la capa Application: MediatR (CQRS), FluentValidation y Mapster.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra en el contenedor de DI los handlers de MediatR, el comportamiento de validación,
    /// los validadores de FluentValidation y la configuración de mapeo de Mapster.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(assembly);

        var mapperConfig = new TypeAdapterConfig();
        mapperConfig.Scan(assembly);
        services.AddSingleton(mapperConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
