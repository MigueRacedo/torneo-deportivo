# HANDOFF — Sistema de Gestión de Torneos de Taekwondo

> Documento de traspaso para retomar el trabajo en una sesión nueva.
> **Última actualización:** 2026-08-20 · Rama de trabajo: `Dev`

---

## Estado de verificación E2E

**H0005 está verificado end-to-end** (2026-08-20): se creó un torneo de cero con 4 categorías y 15
competidores, se generaron llaves y se registraron ganadores desde la UI. Las tres migraciones
(`CategoriaRangosOpcionales`, `CompetidorSinCategoriaYConSexo`, `CategoriaLlavesGeneradas`) están
**aplicadas** — `Program.cs` corre `MigrateAsync()` al arrancar.

Docker (`torneo_db` PostgreSQL) suele estar arriba; si no, `docker-compose up -d`.

> ⚠️ Si se toca el backend con la API corriendo bajo el debugger, `dotnet build` falla con
> **MSB3021/MSB3027** (bloqueo de archivo). No son errores de código: hay que parar el debugger.
> Para ver solo errores reales: `dotnet build 2>&1 | grep ": error CS"`.

---

## Correcciones de H0005 ya aplicadas (2026-08-20)

| # | Problema | Causa real | Fix |
|---|----------|-----------|-----|
| 1 | `POST /llaves/generar` devolvía **415** | El endpoint tenía DTO de request; en un POST FastEndpoints exige body JSON, y el cliente no manda `Content-Type` sin body (regla 9) | `GenerarLlavesEndpoint` pasó a `EndpointWithoutRequest` + `Route<Guid>("torneoId")`; se borró `GenerarLlavesRequest` |
| 2 | Clic en un competidor del bracket no hacía nada | React Flow aplica `pointer-events: none` al nodo cuando `nodesDraggable`, `nodesConnectable` y `elementsSelectable` son `false` y no hay handlers (ver `hasPointerEvents` en su fuente) | `pointer-events-auto` en el div raíz de `MatchNode` |
| 3 | No había vista de resultado tras generar | — | Nueva vista consolidada `/torneos/:id/llaves` (ver `frontend/CLAUDE.md`) |

---

## Estado del proyecto

- **Backend:** compila con 0 errores C# · **90/90 tests** en verde (`cd backend && dotnet test`).
- **Frontend:** `npm run build` OK (`tsc + vite`) · `npm run lint` sin errores
  (1 warning preexistente en `CategoriaForm` por `watch()` de React Hook Form).

### Credenciales seed (usuario demo)
- **Email:** `coordinador@torneo.test` · **Password:** `Coordinador123!` · Rol: Coordinador
- (Definido en `Infrastructure/Persistence/DbSeeder.cs`.)

---

## Historias implementadas

| ID | Título | Estado | Doc |
|----|--------|--------|-----|
| H0001 | Crear torneo | ✅ (venía del starter) | — |
| H0002 | Categorías (CRUD: crear/listar/editar/eliminar) | ✅ | `docs/H0002-definir-categorias.pdf` |
| H0003 | Editar torneo | ✅ | `docs/H0003-editar-torneo.pdf` |
| H0004 | Cargar competidores | ✅ | `docs/H0004-cargar-competidores.pdf` |
| H0005 | **Generar llaves** ⭐ | ✅ verificado E2E | `docs/H0005-generar-llaves.pdf` |

**Bloque CRUD extra** (pedido por el usuario, antes de H0005): editar competidor, eliminar competidor,
eliminar categoría, eliminar torneo (solo Borrador). Todo con **borrado físico + guards por estado + confirmación**.
Documentado en `docs/CRUD-extra-edicion-y-borrado.pdf`.

---

## Pendiente (próximas historias)

| ID | Título | Épica | Puntos | Rol |
|----|--------|-------|--------|-----|
| H0006 | Ver categorías (Profesor) | Consulta Profesor | 1 | Profesor |
| H0007 | Ver competidores (Profesor) | Consulta Profesor | 2 | Profesor |
| H0008 | Generar reporte PDF | Reportes Director | 8 | Director |
| H0009 | **Ciclo de vida del torneo** | Gestión del torneo | 8 | Coordinador |
| H0010 | Rehacer llaves de una categoría | Inscripción y llaves | 5 | Coordinador |
| H0011 | Doble participación (Combate + Formas) | Inscripción y llaves | 13 | Coordinador |
| H0012 | Categoría desierta | Inscripción y llaves | 3 | Coordinador |

- H0007: en backend **solo** existe `ICompetidorRepository.GetByEscuelaAsync` (repositorio + interfaz).
  **No existe el endpoint** `GET /torneos/{torneoId}/competidores/escuela/{escuela}` (rol Profesor) — hay que
  crearlo, junto con su query handler y la vista Profesor en el frontend.
- H0008: usar **QuestPDF** (ya está la licencia Community configurada en `Infrastructure/DependencyInjection.cs`).

### Orden recomendado: H0009 → H0010 → H0012 → H0011

No es el orden en que surgieron sino el de sus dependencias. Ver el detalle del razonamiento y el modelo
de estados en `CLAUDE.md` (raíz), sección **"Ciclo de vida del torneo (H0009)"**.

---

### H0009 — Ciclo de vida del torneo (estados) · 8 pts · Básico

**Por qué primero:** define *cuándo* algo se puede modificar. Sin estados, "editar una categoría con llaves
generadas" (H0010) no tiene respuesta segura: durante la planificación es deseable, durante la competencia
es corrupción de datos. H0009 traza esa línea y **activa todos los guards latentes** que ya están escritos
y testeados pero hoy son inalcanzables.

- Endpoint nuevo: transición de estado del torneo (`Borrador → Activo → Finalizado`).
- Reemplazar el bool `Categoria.LlavesGeneradas` por un enum `EstadoCategoria`
  (`SinLlaves → LlavesGeneradas → EnCurso → Finalizada`) → **migración**.
- `EstadoTorneo` y `EstadoLlave` ya existen en `Domain/Enums/`; falta solo el de categoría y las transiciones.
- Reglas de transición propuestas en `CLAUDE.md` (raíz).
- ⚠️ Todos los guards de "torneo Finalizado" ya existentes pasan a ser alcanzables: revisar que los
  mensajes 409 sean correctos y agregar tests de los caminos que hoy no se pueden ejercitar.

### H0010 — Rehacer llaves de una categoría · 5 pts · Performance

**Origen:** pedido del usuario — "para una categoría con llave ya generada, poder editarla por si necesito
incluir a alguno de los que quedaron fuera".

- Endpoint nuevo: borrar las llaves de una categoría → vuelve a `SinLlaves` y se desbloquea editar/eliminar.
- Decidir si al borrar se **desclasifican** los competidores (`categoria_id → NULL`) para que la próxima
  generación los reparta de nuevo. Probablemente sí, si no la re-clasificación no surte efecto.
- Solo permitido con el torneo en `Borrador` (guard de H0009) y sin ganadores registrados.
- Contradice la regla 4 vigente ("no regenerar llaves") de `skills/bracket-algorithm.md`:
  **hay que actualizar el skill** para acotar la regla a "no regenerar mientras el torneo está Activo".

### H0011 — Doble participación: Combate y Formas · 13 pts · Atractivo

**Origen:** pedido del usuario — "los competidores pueden competir tanto en su categoría de formas como en
la de combate".

- **Cambio de modelo de datos**: competidor ↔ categoría pasa de 1—N a **N—N** (tabla `competidor_categorias`).
  Es lo que lo vuelve caro y lo que obliga a dejarlo último.
- El clasificador deja de ser *first-fit único*: debe asignar la mejor categoría de **Combate** y la mejor de
  **Formas** por separado.
- Impacto en cadena: `Competidor.CategoriaId`, `CompetidorResponse`, `ClasificadorCompetidores`,
  `GenerarLlavesCommandHandler`, la FK `SET NULL` de categoría→competidores, la vista de competidores,
  la vista consolidada de llaves y los reportes de H0008.
- La decisión 2 del §"Decisiones de diseño" de este documento queda **derogada** cuando esto se implemente.

### H0012 — Categoría desierta · 3 pts · Performance

**Origen:** pedido del usuario — "si un competidor no entra en ninguna categoría, poder crearle una donde
está solo (categoría desierta)".

- Flujo para crear una categoría a medida de un competidor que no encajó, desde la vista consolidada
  (la lista de "Competidores sin categoría" ya muestra sus 4 atributos clasificatorios para esto).
- Hoy el backend ya trata a una categoría de 1 competidor como *"campeón directo"* y **no le genera bracket**
  (`GenerarLlavesCommandHandler`); falta representarlo explícitamente como estado/resultado en vez de
  como un caso sin llaves, y que el competidor figure como campeón.
- Depende de H0010 (crear la categoría implica volver a generar).

---

## Decisiones de diseño importantes (contexto que no está en el código)

1. **Competidor desacoplado de categoría.** El competidor se carga **sin categoría** (`categoria_id` nullable)
   y se le agregó **`sexo`**. La relación competidor↔categoría se resuelve al **generar llaves (H0005)**,
   clasificándolo por atributos (sexo/edad/peso/graduación) contra los rangos. Fue una decisión del usuario
   (evita elegir categoría a mano sin validar el encaje).

2. **Clasificación first-fit.** Al generar, cada competidor sin categoría va a la **primera categoría que encaja**.
   Un competidor puede encajar en varias (rangos solapados, o Combate+Formas). Se ajusta editando categorías/competidores
   **antes** de generar. → **Queda derogada por H0011**, que pasa la relación a N—N y permite doble participación.

3. **Borrado físico + guards por estado** (no soft-delete):
   - Eliminar torneo: **solo en Borrador**.
   - Eliminar/editar competidor y categoría: torneo **no Finalizado**.
   - Categoría con **llaves generadas**: bloqueada para editar/eliminar (guard backend + oculto en UI).
   - FKs: torneo→categorías/competidores/llaves en **CASCADE**; categoría→competidores en **SET NULL**.

4. **No regenerar llaves** (regla del skill). Una categoría con `LlavesGeneradas=true` no se regenera.
   Hoy **no hay "borrar llaves"** → si la clasificación quedó mal, no se puede rehacer.
   → **Lo resuelve H0010**, que además obliga a reescribir esa regla del skill.

5. **Transición de estados del torneo NO existe todavía.** Todos los torneos quedan en `Borrador`
   (no hay endpoint para pasar a Activo/Finalizado). Por eso los guards de "Finalizado" son **latentes**
   (correctos pero inalcanzables hoy). Generar llaves **no** cambia el estado. → **Lo resuelve H0009.**

7. **Dos vistas de llaves, a propósito** (decisión del usuario, 2026-08-20):
   - **Unitaria** `/torneos/:id/categorias/:catId/llaves` — un bracket solo, para operar concentrado.
   - **Consolidada** `/torneos/:id/llaves?categoria=<id>` — todas las categorías + las que quedaron sin
     llaves con su motivo + los competidores sin categoría.
   Ambas montan el mismo `BracketPanel`, así que no hay lógica duplicada. Se navega de una a la otra en
   los dos sentidos. **No unificarlas**: sirven a usos distintos (operar vs. auditar).

6. **Bug ya resuelto (no reintroducir):** el fetch wrapper (`frontend/src/lib/api.ts`) solo manda
   `Content-Type: application/json` **cuando hay body** (un GET/DELETE con ese header rompe FastEndpoints con
   "One or more errors occurred!"). Y parsea los errores con `extraerMensajeError` (FastEndpoints devuelve
   `errors` como **objeto** `{campo:[msg]}`, no array). Ver reglas 9-10 de `frontend/CLAUDE.md`.

---

## Convenciones del proyecto (respetarlas)

- **Backend:** Clean Architecture + CQRS (MediatR), **FastEndpoints** (no Controllers), **Mapster** (`.Adapt<T>()`),
  **FluentValidation**, EF Core + Npgsql. Excepciones tipadas: `NotFoundException` (404), `ConflictException` (409).
- **Comentarios XML `/// <summary>`** en español en toda clase/interfaz/método público nuevo (ver `backend/CLAUDE.md`).
- **Reglas compartidas de validación** vía interfaz + extension method: `ICategoriaData`/`CategoriaValidationRules`
  y `ICompetidorData`/`CompetidorValidationRules` (evita duplicar entre Command y Request validators).
- **Frontend:** React 19 + Vite + TS. Server state **solo** en TanStack Query (query keys centralizadas por entidad,
  mutaciones invalidan en `onSuccess`). React Hook Form + Zod. Rutas protegidas con `RoleGuard`.
- **Componente reutilizable `ConfirmDialog`** (`components/ui/confirm-dialog.tsx`, Base UI AlertDialog) para todo
  borrado. Los botones "Eliminar" van **atenuados (`text-muted-foreground`) en reposo → rojo en hover/focus**
  (regla 60-30-10: rojo solo para CTAs).
- **UX/accesibilidad (WCAG 2.2 AA) obligatorio** — ver `skills/ux-ui-guidelines.md`. Targets ≥44px, `<Label>` visible,
  focus visible, errores accionables, HTML semántico, headings sin saltos.
- **Cada historia:** al terminar, se genera un **PDF de decisiones** en `docs/` (se hace con Chrome/Edge headless:
  escribir HTML en scratchpad → `--headless --print-to-pdf`).

---

## Skills a leer según el área (obligatorio antes de tocar)

- `skills/bracket-algorithm.md` — algoritmo de llaves (H0005).
- `skills/react-flow-bracket.md` — visualización del bracket.
- `skills/database-schema.md` — schema de BD (⚠️ ya actualizado con competidor sexo/categoría nullable).
- `skills/ux-ui-guidelines.md` — **obligatorio** antes de tocar cualquier componente visual.
- `skills/api-patterns.md` — patrones de API.
- `CLAUDE.md` (raíz), `backend/CLAUDE.md`, `frontend/CLAUDE.md`.

---

## Arquitectura de H0005 (por si se retoma el bracket)

- **Algoritmo:** `Application/Features/Llaves/BracketGeneratorService.cs` (función pura, 10 tests obligatorios ✅).
- **Clasificador:** `Application/Features/Llaves/ClasificadorCompetidores.cs` (`Encaja(competidor, categoria)`).
- **Casos de uso:** `Features/Llaves/Commands/{GenerarLlaves,RegistrarGanador}` y `Queries/GetBracket`.
- **Endpoints:** `API/Endpoints/Llaves/` (generar POST, ver GET, ganador PUT).
- **SignalR:** `IBracketNotifier` (Application) ← `SignalRBracketNotifier` (API, usa `BracketHub`).
  Evento `MatchActualizado` al grupo `torneo-{id}`. Registrado en `Program.cs`.
- **Frontend:** `components/bracket/` (bracketLayout, MatchNode, BracketView, **BracketPanel**,
  **CategoriaLlaveNav**, **CompetidoresSinCategoria**, GenerarLlavesButton), `hooks/useBracket.ts`,
  `lib/signalr.ts`, y **dos rutas**: `routes/torneos/$torneoId/llaves.tsx` (consolidada) y
  `routes/torneos/$torneoId/categorias/$categoriaId/llaves.tsx` (unitaria).
- `BracketPanel` concentra la lógica de registrar ganador y lo montan **ambas** rutas: si cambia el flujo
  del ganador, se toca un solo archivo.
- La suscripción SignalR vive en `useBracketLiveUpdates(torneoId)` e invalida con `bracketKeys.byTorneo`
  (no `detail`): el evento es de alcance torneo y la conexión no debe reabrirse al cambiar de categoría.

---

## Cómo correr

```bash
# Infra (PostgreSQL, Redis, MinIO)
docker-compose up -d

# Backend  (aplica migraciones al arrancar)
cd backend && dotnet run          # http://localhost:5000

# Frontend
cd frontend && npm run dev        # http://localhost:5173

# Tests backend
cd backend && dotnet test

# Ver solo errores reales de compilación (ignorando bloqueos por API corriendo)
cd backend && dotnet build 2>&1 | grep ": error CS"
```

## Cómo verificar H0005 end-to-end

1. Login como Coordinador. Crear/usar un torneo en Borrador.
2. Crear ≥2 **categorías** con rangos (ej. una Combate Masculino 12–14, cinturón Blanco–Verde).
3. Cargar **competidores** (≥2 que encajen en una categoría: mismo sexo, edad/peso/graduación en rango).
4. En la página de Categorías → botón **"Generar llaves"** → toast con resumen y navega a la vista consolidada.
5. En la vista consolidada: navegar entre categorías, ver las que quedaron sin llaves con su motivo y
   los competidores sin categoría con sus 4 atributos clasificatorios.
6. En la card de la categoría → **"Ver llaves →"** → vista unitaria del bracket (React Flow, pan/zoom).
7. Clic en un competidor de un match → se marca **ganador** y avanza de ronda (y otros clientes lo ven en vivo).

---

## Verificaciones útiles ya hechas (curl, para reproducir)

El backend responde en `:5000`. Para probar por API directa:
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login -H "Content-Type: application/json" \
  -d '{"email":"coordinador@torneo.test","password":"Coordinador123!"}' | sed -E 's/.*"token":"([^"]+)".*/\1/')
curl -s http://localhost:5000/api/v1/torneos -H "Authorization: Bearer $TOKEN"
# ⚠️ En GET NO mandar Content-Type (rompe FastEndpoints). El frontend ya lo maneja.
```

---

## Follow-ups anotados (deuda técnica / mejoras)

> Los tres follow-ups grandes que estaban acá (borrar/regenerar llaves, transición de estados y
> multi-categoría) **se promovieron a historias**: H0010, H0009 y H0011 respectivamente. Ver
> §"Pendiente (próximas historias)".

Queda como deuda suelta:

- Al eliminar un **competidor que ya está en un match**: la FK de `llaves_competencia` es `Restrict` → daría error.
  Falta guard aplicativo (hoy sale como error crudo de EF en vez de un 409 con mensaje claro).
  **Ya no es latente**: con H0005 verificado E2E hay llaves con datos reales, así que es reproducible.
- Validación de encaje del competidor en su categoría también en edición (hoy la clasificación es solo al generar).
- Nombres de propiedad en errores de validación de rangos aparecen como `RangoEdadMin.Value` (cosmético).
- `frontend`: 1 warning de lint preexistente en `CategoriaForm` (`watch()` de React Hook Form es
  incompatible con el React Compiler y saltea la memoización del componente).
