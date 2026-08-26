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
- id, torneoId, categoriaId (**nullable**), nombre, apellido, **sexo (M/F)**, edad, graduacion,
  peso, altura, escuela, responsable, telefono
- Se carga **sin categoría** (H0004); la categoría se asigna al armar las llaves (H0005),
  clasificándolo por sus atributos (sexo, edad, peso, graduación) contra los rangos de cada categoría.
- ⚠️ **H0011 cambia esto a N—N** (tabla `competidor_categorias`): un competidor podrá competir a la vez
  en su categoría de Combate y en la de Formas. `categoriaId` único desaparece.

### LlaveCompetencia (Bracket)
- id, categoriaId, ronda, posicion, competidor1Id, competidor2Id,
  ganadorId, estado

## Historias de Usuario
| ID    | Título                  | Épica                | Puntos | Prioridad KANO | Estado |
|-------|-------------------------|----------------------|--------|----------------|--------|
| H0001 | Crear torneo            | Config. inicial      | 2      | Básico         | ✅ |
| H0002 | Definir categorías      | Config. inicial      | 5      | Básico         | ✅ |
| H0003 | Editar torneo           | Config. inicial      | 3      | Performance    | ✅ |
| H0004 | Cargar competidores     | Inscripción y llaves | 3      | Básico         | ✅ |
| H0005 | Generar llaves          | Inscripción y llaves | 13     | Atractivo ⭐   | ✅ |
| H0006 | Ver categorías (Profesor)| Consulta Profesor    | 1      | Básico         | ✅ |
| H0007 | Ver competidores (Profesor)| Consulta Profesor  | 2      | Performance    | Pendiente |
| H0008 | Generar reporte PDF     | Reportes Director    | 8      | Performance    | Pendiente |
| H0009 | **Ciclo de vida del torneo** | Gestión del torneo | 8   | Básico         | Pendiente |
| H0010 | Rehacer llaves de una categoría | Inscripción y llaves | 5 | Performance | Pendiente |
| H0011 | Doble participación (Combate + Formas) | Inscripción y llaves | 13 | Atractivo | Pendiente |
| H0012 | Categoría desierta      | Inscripción y llaves | 3      | Performance    | Pendiente |
| H0013 | Tablas de competidores filtrables/ordenables/paginadas | Usabilidad | 5 | Performance | Pendiente |

> 📋 **Las especificaciones completas de las historias pendientes (H0007–H0013) y la deuda técnica
> anotada están en `docs/backlog.md`.** Leerlo antes de implementar cualquiera de ellas.

> Además hay un **bloque CRUD extra** (editar/eliminar competidor, eliminar categoría, eliminar torneo)
> hecho entre H0004 y H0005, que no es una historia del backlog.
> Documentado en `docs/CRUD-extra-edicion-y-borrado.pdf`.

### Orden recomendado de las historias nuevas
**H0009 → H0010 → H0012 → H0011.** No es el orden en que surgieron, sino el de sus dependencias:

- **H0009 primero** porque define *cuándo* algo se puede modificar. Sin estados, "editar una categoría
  con llaves generadas" (H0010) no tiene una respuesta segura: mientras el torneo se planifica es
  deseable, mientras se compite es corrupción de datos. H0009 es lo que traza esa línea.
- **H0010 después** porque es el bucle de corrección: sin poder deshacer una generación, un error de
  clasificación es permanente.
- **H0012 se apoya en H0010** (crear una categoría a medida implica rehacer la generación).
- **H0011 al final** porque es el único que cambia el modelo de datos (competidor ↔ categoría pasa de
  1—N a N—N) y toca clasificador, brackets, reportes y toda la UI. Hacerlo antes obligaría a rehacerlo
  encima de cada historia anterior.
- **H0013 es independiente** de las otras cuatro: no comparte código con ellas y se puede hacer en
  cualquier momento. La única atadura es que su columna/filtro de "Categoría" se rehace con H0011
  (una categoría pasa a ser varias).

## Ciclo de vida del torneo (H0009)

Hoy el sistema modela **estados pero no transiciones**: `EstadoTorneo` existe (Borrador/Activo/Finalizado)
pero no hay endpoint que las dispare, así que todos los torneos quedan en `Borrador` para siempre y los
guards que dependen de "Finalizado" son correctos pero inalcanzables. H0009 cierra ese hueco.

La idea rectora es que un torneo atraviesa **dos momentos con reglas opuestas**:

| Momento | Estado | Qué se puede hacer | Qué está bloqueado |
|---------|--------|--------------------|--------------------|
| **Planificación** | `Borrador` | Crear/editar/eliminar categorías y competidores, generar y **rehacer** llaves | Registrar ganadores |
| **Competencia** | `Activo` | Registrar ganadores, avanzar rondas | Tocar categorías, competidores o llaves |
| **Cierre** | `Finalizado` | Consultar y generar reportes (H0008) | Toda escritura |

**Tres niveles de estado, no uno:**
- **Torneo** — `EstadoTorneo` (ya existe): Borrador → Activo → Finalizado.
- **Categoría** — hoy solo el bool `LlavesGeneradas`. H0009 lo reemplaza por un enum `EstadoCategoria`
  (`SinLlaves` → `LlavesGeneradas` → `EnCurso` → `Finalizada`), que permite saber si una categoría ya
  tiene campeón sin recorrer todos sus matches.
- **Match** — `EstadoLlave` (ya existe): Pendiente / EnCurso / Finalizado / Bye.

**Reglas de transición a definir en la historia:**
- Borrador → Activo: exige al menos una categoría con llaves generadas.
- Activo → Finalizado: exige que todas las categorías estén Finalizadas (o forzado por el Coordinador).
- Activo → Borrador: solo si todavía no se registró ningún ganador (si no, habría que descartar resultados).
- El torneo pasa a Finalizado **solo** desde Activo; nunca se salta un estado.

## Decisiones de diseño vigentes (contexto que no está en el código)

1. **Competidor desacoplado de categoría.** Se carga **sin categoría** (`categoria_id` nullable) y con
   **`sexo`**. La relación se resuelve al **generar llaves (H0005)**, clasificándolo por atributos contra
   los rangos. Evita elegir categoría a mano sin validar el encaje.

2. **Clasificación first-fit.** Cada competidor sin categoría va a la **primera categoría que encaja**, y
   solo se consideran categorías **sin llaves generadas** (si no, quedaría con categoría pero fuera del
   bracket, e invisible). → Queda derogada por **H0011**, que pasa la relación a N—N.

3. **Borrado físico + guards por estado** (no soft-delete). Los guards viven en el Command Handler y
   devuelven **409**:
   - Eliminar torneo: solo en **Borrador**.
   - Editar/eliminar competidor y categoría: torneo **no Finalizado**.
   - Categoría con **llaves generadas**: bloqueada para editar/eliminar (guard backend + oculta en UI).
   - Todas verifican además que el recurso pertenezca al torneo de la ruta → **404**, no 403 (no filtrar
     la existencia de recursos de otros torneos).
   - FKs: torneo→categorías/competidores/llaves en **CASCADE**; categoría→competidores en **SET NULL**;
     competidor→llaves en **RESTRICT**.

4. **No regenerar llaves.** Una categoría con `LlavesGeneradas=true` no se regenera y hoy no hay "borrar
   llaves". → Lo resuelve **H0010**, que obliga a reescribir esa regla de `skills/bracket-algorithm.md`.

5. **Las transiciones de estado del torneo NO existen todavía.** Todos quedan en `Borrador`; generar llaves
   no cambia el estado. Por eso los guards de "Finalizado" son **latentes** (correctos pero inalcanzables).
   → Lo resuelve **H0009**.

6. **Un match no se reabre.** Registrar ganador exige match con sus **dos competidores** definidos y estado
   distinto de Finalizado/Bye: cambiar un ganador después de que avanzó la ronda siguiente dejaría al
   perdedor propagado aguas abajo.

## Usuarios demo (seed)

- **Coordinador:** `coordinador@torneo.test` · `Coordinador123!`
- **Profesor:** `profesor@torneo.test` · `Profesor123!`

Definidos en `Infrastructure/Persistence/DbSeeder.cs`, que verifica **por email, uno por uno**: una base
existente recibe los roles nuevos al reiniciar, sin recrearla. ⚠️ El sembrado corre **solo en Development**
(`app.Environment.IsDevelopment()` en `Program.cs`) porque estas contraseñas están publicadas en el repo;
las migraciones sí se aplican en todos los entornos.

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

### Documentación de cada historia
- Al terminar una historia se genera un **PDF de decisiones** en `docs/` (escribir el HTML en el
  scratchpad y renderizarlo con Chrome/Edge `--headless --print-to-pdf`).
- Si la historia cambia una decisión de diseño vigente, actualizar también la sección de arriba.

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
