# Sistema de Gestión de Torneos Deportivos de Contacto 🥋

## Descripción del Proyecto
Sistema web para automatizar el armado de llaves y categorías en torneos deportivos
de contacto (Taekwondo) en Argentina. Reemplaza el proceso manual en papel.

## Stack Tecnológico (2026)
- **Frontend**: React 19 + Vite + TypeScript
- **Estado/Data**: TanStack Query (server state) + Zustand (client state)
- **UI**: Tailwind CSS + shadcn/ui
- **Bracket**: React Flow (@xyflow/react) — visualización del árbol de llaves
- **Forms**: React Hook Form + Zod
- **Routing**: TanStack Router (type-safe) o React Router v7
- **Backend**: .NET 10 Web API + C# 14
- **Estilo de API**: FastEndpoints (en lugar de Controllers MVC)
- **Patrón**: CQRS con MediatR
- **Validación**: FluentValidation
- **Mapeo**: Mapster (más rápido y simple que AutoMapper)
- **ORM**: Entity Framework Core 10 + Npgsql
- **PDF**: QuestPDF (reportes del Director)
- **Tiempo real**: SignalR (actualización de brackets en vivo)
- **Base de Datos**: PostgreSQL 16
- **Cache**: Redis (opcional — cache de brackets generados)
- **Storage**: MinIO / S3 (imágenes de flyers)
- **Auth**: JWT Bearer Tokens
- **Contenedores**: Docker + Docker Compose
- **CI/CD**: GitHub Actions
- **Deploy**: Fly.io o Railway

## Por qué este stack
- **React + Vite, no Next.js**: es un dashboard privado (no necesita SSR ni SEO).
  Vite es más rápido en dev y el deploy es un SPA estático simple.
- **FastEndpoints, no Controllers**: menos boilerplate, mejor performance,
  una clase por endpoint = más alineado con Clean Architecture.
- **TanStack Query**: maneja cache, refetch y sincronización automáticamente.
  Elimina los hooks manuales de loading/error/data.
- **React Flow**: librería específica para grafos/árboles. Es la mejor herramienta
  para renderizar el bracket (H0005) de forma interactiva.
- **Mapster, no AutoMapper**: API más simple, source-generated, sin reflexión en runtime.
- **QuestPDF**: API fluent en C#, PDFs profesionales, open source.

## Arquitectura
```
torneo-deportivo/
├── frontend/          # React 19 + Vite
├── backend/           # .NET 10 Web API (FastEndpoints)
├── database/          # Migraciones y seeds
├── skills/            # Guías especializadas para Claude
├── .claude/
│   └── commands/      # Slash commands reutilizables
├── docker-compose.yml
└── CLAUDE.md          ← estás aquí
```

## Roles de Usuario
| Rol          | Descripción                                    | Permisos clave                          |
|-------------|------------------------------------------------|----------------------------------------|
| Coordinador  | Gestiona torneos, categorías y competidores    | CRUD torneos, CRUD categorías, cargar competidores, generar llaves |
| Profesor     | Consulta info y sus alumnos inscriptos         | Ver torneos, ver categorías, ver sus competidores |
| Director     | Visión global y reportes                       | Ver métricas, generar reportes PDF      |

## Entidades Principales
```
Torneo → Categorías → Competidores → Llaves (Bracket)
```

### Torneo
- id, nombre, fecha, lugar, imagenFlyer, estado (Borrador/Activo/Finalizado)

### Categoría
- id, torneoId, nombre, tipoCompetencia (Combate/Formas), sexo (M/F),
  rangoEdadMin, rangoEdadMax, rangoPesoMin, rangoPesoMax,
  rangoGraduacionMin, rangoGraduacionMax

### Competidor
- id, torneoId, categoriaId, nombre, apellido, edad, graduacion,
  peso, altura, escuela, responsable, telefono

### LlaveCompetencia (Bracket)
- id, categoriaId, ronda, posicion, competidor1Id, competidor2Id,
  ganadorId, estado

## Historias de Usuario Implementadas
| ID    | Título                  | Épica                | Puntos | Prioridad KANO |
|-------|-------------------------|----------------------|--------|----------------|
| H0001 | Crear torneo            | Config. inicial      | 2      | Básico         |
| H0002 | Definir categorías      | Config. inicial      | 5      | Básico         |
| H0003 | Editar torneo           | Config. inicial      | 3      | Performance    |
| H0004 | Cargar competidores     | Inscripción y llaves | 3      | Básico         |
| H0005 | Generar llaves          | Inscripción y llaves | 13     | Atractivo ⭐   |
| H0006 | Ver categorías (Profesor)| Consulta Profesor    | 1      | Básico         |
| H0007 | Ver competidores (Profesor)| Consulta Profesor  | 2      | Performance    |
| H0008 | Generar reporte PDF     | Reportes Director    | 8      | Performance    |

## Convenciones de Código

### Backend (.NET 10)
- Arquitectura: Clean Architecture (Domain / Application / Infrastructure / API)
- Endpoints: FastEndpoints (una clase `Endpoint<Request, Response>` por operación)
- Patrón interno: CQRS con MediatR (Commands para escritura, Queries para lectura)
- Validación: FluentValidation (también integrada en FastEndpoints)
- Mapeo: Mapster
- Nomenclatura: PascalCase para clases, camelCase para variables
- DTOs: sufijo `Request` para entrada, `Response` para salida
- Rutas: `/api/v1/[entidad]`

### Frontend (React + Vite)
- Componentes: PascalCase, sufijo según tipo (e.g. `TorneoCard.tsx`)
- Server state: **TanStack Query** — un hook por recurso (e.g. `useTorneos`)
- Client state: **Zustand** — solo para auth y UI state global
- API client: `src/lib/api.ts` con fetch wrapper tipado
- Forms: React Hook Form + Zod
- UI Components: shadcn/ui + Tailwind
- Bracket: React Flow en `src/components/bracket/`

### Base de Datos
- Tablas: snake_case en plural (e.g. `torneos`, `categorias`)
- PKs: `id` (UUID)
- FKs: `{entidad}_id`
- Timestamps: `created_at`, `updated_at` en todas las tablas

## Comandos Frecuentes
```bash
# Levantar todo el entorno
docker-compose up -d

# Backend
cd backend && dotnet run

# Frontend
cd frontend && npm run dev      # Vite dev server (puerto 5173)

# Migraciones
cd backend && dotnet ef migrations add {NombreMigracion}
cd backend && dotnet ef database update

# Tests
cd backend && dotnet test
cd frontend && npm run test     # Vitest
```

## Skills Especializadas
Antes de trabajar en áreas específicas, leer el skill correspondiente:
- Algoritmo de llaves → `skills/bracket-algorithm.md`
- Patrones de API → `skills/api-patterns.md`
- Schema de BD → `skills/database-schema.md`
- Bracket con React Flow → `skills/react-flow-bracket.md`
- UX/UI y Accesibilidad (WCAG 2.2 AA) → `skills/ux-ui-guidelines.md` — **obligatorio**
  antes de crear o tocar cualquier componente visual del frontend

## Reglas Importantes
1. **Nunca** hacer consultas SQL directas fuera del repositorio EF
2. **Siempre** validar entrada en el backend (FluentValidation), aunque el frontend también valide
3. El algoritmo de bracket (H0005) es el core del proyecto — revisar `skills/bracket-algorithm.md` antes de modificarlo
4. Los archivos de migración de BD son inmutables una vez aplicados en producción
5. Todo endpoint nuevo requiere su test de integración correspondiente
6. Las llaves generadas son de **eliminación directa** (single-elimination bracket)
7. El estado del servidor SIEMPRE vive en TanStack Query, nunca duplicarlo en Zustand
8. Siempre que terminas de impementar/escribir codigo de algo que te pido, compilalo y fijate que no haya errores. si hay errores, corregelos y vuelve a compilar.
