using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using QuestPDF.Infrastructure;
using TorneoDeportivo.Application.Common.Interfaces;
using TorneoDeportivo.Domain.Interfaces;
using TorneoDeportivo.Infrastructure.Persistence;
using TorneoDeportivo.Infrastructure.Persistence.Repositories;
using TorneoDeportivo.Infrastructure.Services;

namespace TorneoDeportivo.Infrastructure;

/// <summary>
/// Registro de servicios de la capa Infrastructure: EF Core (PostgreSQL), repositorios, MinIO y QuestPDF.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra en el contenedor de DI el <see cref="TorneoDbContext"/>, los repositorios, el hasher de contraseñas,
    /// el generador de JWT, el cliente de MinIO y la licencia de QuestPDF.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TorneoDbContext>(options => options
            .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<ITorneoRepository, TorneoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        var minioOptions = new MinioOptions();
        configuration.GetSection("Minio").Bind(minioOptions);

        services.AddSingleton<IMinioClient>(_ => new MinioClient()
            .WithEndpoint(minioOptions.Endpoint)
            .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
            .WithSSL(minioOptions.UseSsl)
            .Build());

        QuestPDF.Settings.License = LicenseType.Community;

        return services;
    }
}
