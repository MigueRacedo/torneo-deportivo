# Backend — .NET 10 Web API (FastEndpoints)

> Este CLAUDE.md aplica solo dentro de `/backend`. Se complementa con el CLAUDE.md raíz.

## Estructura del Proyecto
```
backend/
├── TorneoDeportivo.Domain/          # Entidades, interfaces, enums
│   ├── Entities/
│   ├── Enums/
│   └── Interfaces/
├── TorneoDeportivo.Application/     # Casos de uso, DTOs, validaciones
│   ├── Features/
│   │   ├── Torneos/
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   ├── Categorias/
│   │   ├── Competidores/
│   │   └── Llaves/
│   ├── Services/
│   │   └── BracketGeneratorService.cs   # ⭐ Algoritmo de llaves (H0005)
│   └── Common/
├── TorneoDeportivo.Infrastructure/  # EF Core, repositorios, servicios externos
│   ├── Persistence/
│   │   ├── Configurations/         # Fluent API de EF
│   │   ├── Migrations/
│   │   └── TorneoDbContext.cs
│   └── Services/                    # QuestPDF, MinIO, SignalR hubs
└── TorneoDeportivo.API/             # FastEndpoints, Program.cs, middleware
    ├── Endpoints/
    │   ├── Torneos/
    │   ├── Categorias/
    │   ├── Competidores/
    │   └── Llaves/
    ├── Hubs/                        # SignalR hubs
    └── Program.cs
```

## Por qué FastEndpoints (no Controllers)
FastEndpoints es un framework REST minimalista sobre .NET. Ventajas para este proyecto:
- **Una clase por endpoint** (REPR pattern: Request-Endpoint-Response) → encaja perfecto con CQRS
- **Menos boilerplate** que Controllers MVC
- **Mejor performance** (no usa el pipeline pesado de MVC)
- **Validación integrada** con FluentValidation
- **Generación automática** de Swagger

## Documentación XML (obligatorio)
Toda clase, interfaz, enum y método público nuevo debe incluir un comentario `/// <summary>` en español
en su cabecera, explicando qué hace y su rol en el flujo — para que se entienda sin leer la implementación.
- Ejemplo:
  ```csharp
  /// <summary>
  /// Handler CQRS que procesa el comando de creación de torneo y lo persiste en estado Borrador.
  /// </summary>
  public class CreateTorneoCommandHandler : IRequestHandler<CreateTorneoCommand, TorneoResponse>
  ```
- En records (Request/Response/Command/Query) alcanza con un `<summary>` sobre la declaración del record.
- En entidades de dominio alcanza con un `<summary>` a nivel de clase (no hace falta por cada propiedad).
- Usar `<param>` / `<returns>` solo cuando aportan valor real (parámetros no obvios).

## Patrones Obligatorios

### Endpoint (FastEndpoints) + Command (CQRS)
```csharp
// API/Endpoints/Torneos/CreateTorneoEndpoint.cs
public class CreateTorneoEndpoint : Endpoint<CreateTorneoRequest, TorneoResponse>
{
    private readonly ISender _sender;
    public CreateTorneoEndpoint(ISender sender) => _sender = sender;

    public override void Configure()
    {
        Post("/api/v1/torneos");
        Roles("Coordinador");
        Description(b => b.Produces<TorneoResponse>(201));
    }

    public override async Task HandleAsync(CreateTorneoRequest req, CancellationToken ct)
    {
        var command = new CreateTorneoCommand(req.Nombre, req.Fecha, req.Lugar, req.ImagenFlyer);
        var result = await _sender.Send(command, ct);
        await SendCreatedAtAsync<GetTorneoByIdEndpoint>(
            new { id = result.Id }, result, cancellation: ct);
    }
}

// API/Endpoints/Torneos/CreateTorneoRequest.cs
public record CreateTorneoRequest(string Nombre, DateOnly Fecha, string Lugar, string? ImagenFlyer);

// Validación del Request (FastEndpoints la detecta automáticamente)
public class CreateTorneoRequestValidator : Validator<CreateTorneoRequest>
{
    public CreateTorneoRequestValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Fecha).GreaterThan(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.Lugar).NotEmpty().MaximumLength(300);
    }
}
```

### Command Handler (Application layer)
```csharp
// Application/Features/Torneos/Commands/CreateTorneo/
public record CreateTorneoCommand(string Nombre, DateOnly Fecha, string Lugar, string? ImagenFlyer)
    : IRequest<TorneoResponse>;

public class CreateTorneoCommandHandler : IRequestHandler<CreateTorneoCommand, TorneoResponse>
{
    private readonly ITorneoRepository _repo;
    public CreateTorneoCommandHandler(ITorneoRepository repo) => _repo = repo;

    public async Task<TorneoResponse> Handle(CreateTorneoCommand request, CancellationToken ct)
    {
        var torneo = new Torneo
        {
            Nombre = request.Nombre,
            Fecha = request.Fecha,
            Lugar = request.Lugar,
            ImagenFlyer = request.ImagenFlyer,
            Estado = EstadoTorneo.Borrador
        };
        await _repo.AddAsync(torneo, ct);
        return torneo.Adapt<TorneoResponse>();   // Mapster
    }
}
```

### Query Handler
```csharp
public record GetTorneosQuery : IRequest<List<TorneoResponse>>;

public class GetTorneosQueryHandler : IRequestHandler<GetTorneosQuery, List<TorneoResponse>>
{
    private readonly ITorneoRepository _repo;
    public GetTorneosQueryHandler(ITorneoRepository repo) => _repo = repo;

    public async Task<List<TorneoResponse>> Handle(GetTorneosQuery request, CancellationToken ct)
    {
        var torneos = await _repo.GetAllActivosAsync(ct);
        return torneos.Adapt<List<TorneoResponse>>();
    }
}
```

## Endpoints de la API

### Torneos
| Método | Ruta                          | Descripción              | Rol requerido |
|--------|-------------------------------|--------------------------|---------------|
| GET    | /api/v1/torneos               | Listar torneos activos   | Todos         |
| GET    | /api/v1/torneos/{id}          | Obtener torneo           | Todos         |
| POST   | /api/v1/torneos               | Crear torneo             | Coordinador   |
| PUT    | /api/v1/torneos/{id}          | Editar torneo            | Coordinador   |
| DELETE | /api/v1/torneos/{id}          | Eliminar torneo          | Coordinador   |

### Categorías
| Método | Ruta                                         | Descripción             | Rol requerido |
|--------|----------------------------------------------|-------------------------|---------------|
| GET    | /api/v1/torneos/{torneoId}/categorias         | Listar categorías       | Todos         |
| POST   | /api/v1/torneos/{torneoId}/categorias         | Crear categoría         | Coordinador   |
| PUT    | /api/v1/torneos/{torneoId}/categorias/{id}    | Editar categoría        | Coordinador   |

### Competidores
| Método | Ruta                                            | Descripción                  | Rol requerido |
|--------|-------------------------------------------------|------------------------------|---------------|
| GET    | /api/v1/torneos/{torneoId}/competidores          | Listar todos                 | Coordinador   |
| GET    | /api/v1/torneos/{torneoId}/competidores/escuela/{escuela} | Listar por escuela | Profesor      |
| POST   | /api/v1/torneos/{torneoId}/competidores          | Cargar competidor            | Coordinador   |

### Llaves (Bracket)
| Método | Ruta                                        | Descripción                        | Rol requerido |
|--------|---------------------------------------------|------------------------------------|---------------|
| POST   | /api/v1/torneos/{torneoId}/llaves/generar    | **Generar llaves automáticamente** | Coordinador   |
| GET    | /api/v1/torneos/{torneoId}/llaves/{categoriaId} | Ver bracket de una categoría   | Todos         |
| PUT    | /api/v1/torneos/{torneoId}/llaves/{id}/ganador  | Registrar ganador de un match  | Coordinador   |

### Reportes
| Método | Ruta                                      | Descripción              | Rol requerido |
|--------|-------------------------------------------|--------------------------|---------------|
| GET    | /api/v1/torneos/{torneoId}/reporte/pdf    | Descargar reporte PDF    | Director      |

## Mapeo con Mapster
```csharp
// No requiere configuración para mapeos directos (convención por nombre):
var response = torneo.Adapt<TorneoResponse>();

// Para mapeos custom, registrar en un IRegister:
public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Competidor, CompetidorResponse>()
              .Map(dest => dest.NombreCompleto, src => $"{src.Nombre} {src.Apellido}");
    }
}
```

## Configuración de EF Core (FluentAPI)
```csharp
// Infrastructure/Persistence/Configurations/TorneoConfiguration.cs
public class TorneoConfiguration : IEntityTypeConfiguration<Torneo>
{
    public void Configure(EntityTypeBuilder<Torneo> builder)
    {
        builder.ToTable("torneos");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(t => t.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Lugar).IsRequired().HasMaxLength(300);
        builder.Property(t => t.Estado).HasConversion<string>();
        builder.HasMany(t => t.Categorias).WithOne(c => c.Torneo)
               .HasForeignKey(c => c.TorneoId).OnDelete(DeleteBehavior.Cascade);
    }
}
```

## Reportes PDF con QuestPDF
```csharp
// Infrastructure/Services/ReporteResultadosService.cs
// QuestPDF usa una API fluent declarativa:
public byte[] GenerarReporte(Torneo torneo, List<ResultadoCategoria> resultados)
{
    var doc = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.Header().Text($"Reporte - {torneo.Nombre}").FontSize(20).Bold();
            page.Content().Column(col =>
            {
                col.Item().Text($"Total inscriptos: {resultados.Sum(r => r.TotalCompetidores)}");
                foreach (var cat in resultados)
                {
                    col.Item().Table(table => { /* tabla por categoría */ });
                }
            });
        });
    });
    return doc.GeneratePdf();
}
```

## SignalR — Actualización en Vivo del Bracket
```csharp
// API/Hubs/BracketHub.cs
public class BracketHub : Hub
{
    // Los clientes se unen al grupo de un torneo
    public async Task JoinTorneo(string torneoId)
        => await Groups.AddToGroupAsync(Context.ConnectionId, $"torneo-{torneoId}");
}

// Cuando se registra un ganador, notificar al grupo:
await _hubContext.Clients.Group($"torneo-{torneoId}")
    .SendAsync("MatchActualizado", matchActualizado, ct);
```

## Registro de Servicios (Program.cs)
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();    // MediatR, Mapster, FluentValidation
builder.Services.AddInfrastructure(builder.Configuration);  // EF, repos, QuestPDF, MinIO
builder.Services.AddFastEndpoints();  // FastEndpoints
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = jwtSecret);
builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.SwaggerDocument();   // Swagger via FastEndpoints

var app = builder.Build();
app.UseAuthentication().UseAuthorization();
app.UseFastEndpoints().UseSwaggerGen();
app.MapHub<BracketHub>("/hubs/bracket");
app.Run();
```

## Errores de Manejo
- 400: ValidationException (FluentValidation) — FastEndpoints lo maneja automáticamente
- 401: usuario no autenticado
- 403: rol incorrecto
- 404: NotFoundException
- 409: ConflictException (e.g. torneo ya con llaves generadas)
- 500: handled por middleware global

## Tests
- **Unit tests**: xUnit + NSubstitute para handlers de CQRS y BracketGeneratorService
- **Integration tests**: FastEndpoints tiene `App.Fixture` para testing de endpoints end-to-end
- Convención de nombres: `{Metodo}_{Escenario}_{ResultadoEsperado}`
  - Ejemplo: `Handle_ValidCommand_ReturnsTorneoResponse`
- El `BracketGeneratorService` requiere tests exhaustivos (ver `skills/bracket-algorithm.md`)
