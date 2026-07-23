using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TorneoDeportivo.API.Hubs;
using TorneoDeportivo.API.Middleware;
using TorneoDeportivo.Application;
using TorneoDeportivo.Domain.Interfaces;
using TorneoDeportivo.Infrastructure;
using TorneoDeportivo.Infrastructure.Persistence;

// Punto de entrada de la API: registra los servicios de las capas Application e Infrastructure,
// configura autenticación JWT, CORS, FastEndpoints y SignalR, aplica migraciones y siembra datos iniciales.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFastEndpoints();
builder.Services.AddSignalR();

var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Falta configurar Jwt:Secret");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins(builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:5173")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));

var app = builder.Build();

// Aplica migraciones pendientes de la base de datos y siembra los usuarios iniciales (seed).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TorneoDbContext>();
    await db.Database.MigrateAsync();

    var usuarios = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DbSeeder.SeedAsync(usuarios, passwordHasher);
}

app.UseCors();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication().UseAuthorization();
app.UseFastEndpoints();
app.MapHub<BracketHub>("/hubs/bracket");

app.Run();
