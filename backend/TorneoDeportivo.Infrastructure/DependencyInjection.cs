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

public static class DependencyInjection
{
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
