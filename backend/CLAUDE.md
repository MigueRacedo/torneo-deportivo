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
| DELETE | /api/v1/torneos/{torneoId}/categorias/{id}    | Eliminar categoría      | Coordinador   |

### Competidores
| Método | Ruta                                            | Descripción                  | Rol requerido |
|--------|-------------------------------------------------|------------------------------|---------------|
| GET    | /api/v1/torneos/{torneoId}/competidores          | Listar todos                 | Coordinador   |
| POST   | /api/v1/torneos/{torneoId}/competidores          | Cargar competidor            | Coordinador   |
| PUT    | /api/v1/torneos/{torneoId}/competidores/{id}     | Editar competidor            | Coordinador   |
| DELETE | /api/v1/torneos/{torneoId}/competidores/{id}     | Eliminar competidor          | Coordinador   |

> ⚠️ **Todavía NO implementado:** `GET /api/v1/torneos/{torneoId}/competidores/escuela/{escuela}`
> (listar por escuela, rol Profesor) — pertenece a **H0007**. El repositorio ya expone
> `ICompetidorRepository.GetByEscuelaAsync`, pero **no existe el endpoint** que lo consuma.

### Guards de estado en edición y borrado
Los guards viven en el **Command Handler** (no en el endpoint) y devuelven **409 `ConflictException`**:

| Operación                      | Condición requerida                                     |
|--------------------------------|---------------------------------------------------------|
| Eliminar torneo                | Estado **Borrador**                                      |
| Editar/eliminar categoría      | Torneo **no Finalizado** **y** `LlavesGeneradas == false` |
| Editar/eliminar competidor     | Torneo **no Finalizado**                                 |

Además, **todas** verifican que el recurso pertenezca al torneo de la ruta
(`entidad.TorneoId != request.TorneoId` → **404**, no 403: evita filtrar la existencia de
recursos de otros torneos). Ver `docs/CRUD-extra-edicion-y-borrado.pdf`.

### Llaves (Bracket)
| Método | Ruta                                        | Descripción                        | Rol requerido |
|--------|---------------------------------------------|------------------------------------|---------------|
| POST   | /api/v1/torneos/{torneoId}/llaves/generar    | **Generar llaves automáticamente** | Coordinador   |
| GET    | /api/v1/torneos/{torneoId}/llaves/{categoriaId} | Ver bracket de una categoría   | Todos         |
| PUT    | /api/v1/torneos/{torneoId}/llaves/{id}/ganador  | Registrar ganador de un match  | Coordinador   |

> ⚠️ `POST .../llaves/generar` es un **`EndpointWithoutRequest`** a propósito: no lleva body y el único
> dato entra por la ruta (`Route<Guid>("torneoId")`). Con un DTO de request, FastEndpoints intentaría
> deserializar el body en un POST y devolvería **415** porque el cliente no manda `Content-Type` si no
> hay cuerpo. **Mismo criterio para todo POST/PUT sin body que se agregue.**

### Reportes
| Método | Ruta                                      | Descripción              | Rol requerido |
|--------|-------------------------------------------|--------------------------|---------------|
| GET    | /api/v1/torneos/{torneoId}/reporte/pdf    | Descargar reporte PDF    | Director      |

> ⚠️ **Todavía NO implementado** — es H0008. No existe `Endpoints/Reportes/`.

### Endpoints planificados (historias H0009–H0012)
Ninguno existe todavía; se listan para que la nomenclatura salga consistente cuando se implementen.
El diseño completo está en `CLAUDE.md` (raíz) y `HANDOFF.md`.

| Historia | Método | Ruta (propuesta)                                      | Descripción |
|----------|--------|-------------------------------------------------------|-------------|
| H0009    | PUT    | /api/v1/torneos/{id}/estado                            | Transición Borrador → Activo → Finalizado |
| H0010    | DELETE | /api/v1/torneos/{torneoId}/llaves/{categoriaId}        | Borrar las llaves de una categoría para rehacerlas |

- **H0009** reemplaza el bool `Categoria.LlavesGeneradas` por un enum `EstadoCategoria`
  (`SinLlaves → LlavesGeneradas → EnCurso → Finalizada`) → requiere migración. `EstadoTorneo` y
  `EstadoLlave` ya existen en `Domain/Enums/`.
- **H0011** (doble participación Combate + Formas) cambia el modelo a **N—N** con tabla
  `competidor_categorias`: no agrega endpoints nuevos pero altera `CompetidorResponse`,
  `ClasificadorCompetidores` y `GenerarLlavesCommandHandler`.
- **H0012** (categoría desierta) se apoya en el POST de categorías ya existente; lo nuevo es representar
  al competidor único como campeón en vez de como "categoría sin llaves".

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

### Contrato de respuesta de error (importante para el frontend)
FastEndpoints, ante un fallo de `Validator<T>` **o de binding/deserialización**, responde
400 con esta forma (el `message` es genérico; el detalle va en `errors` como **objeto**):
```json
{ "statusCode": 400, "message": "One or more errors occurred!",
  "errors": { "campo": ["mensaje específico"] } }
```
- El cliente debe leer `errors[campo][0]`, **no** `message` (ver `extraerMensajeError` en `frontend/src/lib/api.ts`).
- ⚠️ Un **GET/DELETE con header `Content-Type: application/json` y sin body** dispara este
  400 (`serializerErrors`: "The input does not contain any JSON tokens"). El cliente solo
  debe mandar `Content-Type` cuando hay body. Ver regla 9 de `frontend/CLAUDE.md`.

## Tests
- **Unit tests**: xUnit + NSubstitute para handlers de CQRS y BracketGeneratorService
- **Integration tests**: FastEndpoints tiene `App.Fixture` para testing de endpoints end-to-end
- Convención de nombres: `{Metodo}_{Escenario}_{ResultadoEsperado}`
  - Ejemplo: `Handle_ValidCommand_ReturnsTorneoResponse`
- El `BracketGeneratorService` requiere tests exhaustivos (ver `skills/bracket-algorithm.md`)
