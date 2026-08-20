# HANDOFF — Sistema de Gestión de Torneos de Taekwondo

> Documento de traspaso para retomar el trabajo en una sesión nueva.
> **Última actualización:** 2026-08-19 · Rama de trabajo: `Dev`

---

## 🔴 Acción crítica pendiente (leer primero)

La **API en ejecución es un build viejo**. Todo lo implementado desde **H0003 en adelante**
(editar torneo, todo el CRUD de borrado, y **H0005 completo**) **no está verificado end-to-end**
porque el proceso `dotnet run` corriendo es anterior a esos cambios.

**Antes de seguir o de probar:**
1. Parar la API (`Ctrl+C` en la terminal del `dotnet run`).
2. Volver a levantarla: `cd backend && dotnet run` (o `docker-compose up -d` + backend).
   - Al arrancar, `Program.cs` corre `MigrateAsync()` y **aplica solas** las migraciones pendientes.
3. Verificar E2E el flujo completo (ver §"Cómo verificar").

**Migraciones generadas pero (probablemente) NO aplicadas todavía en la BD:**
- `CategoriaRangosOpcionales` (edad/peso nullable en categorías)
- `CompetidorSinCategoriaYConSexo` (competidor: `categoria_id` nullable + campo `sexo`)
- `CategoriaLlavesGeneradas` (flag `llaves_generadas`)

Docker (`torneo_db` PostgreSQL) suele estar arriba; si no, `docker-compose up -d`.

---

## Estado del proyecto

- **Backend:** compila con 0 errores C# · **90/90 tests** en verde (`cd backend && dotnet test`).
- **Frontend:** `npm run build` OK (`tsc + vite`).
- Nota: al hacer `dotnet build` con la API corriendo, aparecen errores **MSB3021/MSB3027**
  (bloqueo de archivo) — **no son errores de código**, es el proceso en ejecución. Para ver errores
  reales de compilación: `dotnet build 2>&1 | grep ": error CS"`.

### Credenciales seed (usuario demo)
- **Email:** `coordinador@torneo.test` · **Password:** `Coordinador123!` · Rol: Coordinador
- (Definido en `Infrastructure/Persistence/DbSeeder.cs`.)

---

## Historias implementadas

| ID | Título | Estado | Doc |
|----|--------|--------|-----|
| H0001 | Crear torneo | ✅ (venía del starter) | — |
| H0002 | Categorías (CRUD: crear/listar/editar/eliminar) | ✅ | `docs/H0002-definir-categorias.pdf` |
| H0003 | Editar torneo | ✅ (sin verificar E2E) | `docs/H0003-editar-torneo.pdf` |
| H0004 | Cargar competidores | ✅ (sin verificar E2E) | `docs/H0004-cargar-competidores.pdf` |
| H0005 | **Generar llaves** ⭐ | ✅ (sin verificar E2E) | `docs/H0005-generar-llaves.pdf` |

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

- H0007: en backend **solo** existe `ICompetidorRepository.GetByEscuelaAsync` (repositorio + interfaz).
  **No existe el endpoint** `GET /torneos/{torneoId}/competidores/escuela/{escuela}` (rol Profesor) — hay que
  crearlo, junto con su query handler y la vista Profesor en el frontend.
- H0008: usar **QuestPDF** (ya está la licencia Community configurada en `Infrastructure/DependencyInjection.cs`).

---

## Decisiones de diseño importantes (contexto que no está en el código)

1. **Competidor desacoplado de categoría.** El competidor se carga **sin categoría** (`categoria_id` nullable)
   y se le agregó **`sexo`**. La relación competidor↔categoría se resuelve al **generar llaves (H0005)**,
   clasificándolo por atributos (sexo/edad/peso/graduación) contra los rangos. Fue una decisión del usuario
   (evita elegir categoría a mano sin validar el encaje).

2. **Clasificación first-fit.** Al generar, cada competidor sin categoría va a la **primera categoría que encaja**.
   Un competidor puede encajar en varias (rangos solapados, o Combate+Formas). Se ajusta editando categorías/competidores
   **antes** de generar. Modelar doble participación (Combate Y Formas) es extensión futura (hoy `categoria_id` único).

3. **Borrado físico + guards por estado** (no soft-delete):
   - Eliminar torneo: **solo en Borrador**.
   - Eliminar/editar competidor y categoría: torneo **no Finalizado**.
   - Categoría con **llaves generadas**: bloqueada para editar/eliminar (guard backend + oculto en UI).
   - FKs: torneo→categorías/competidores/llaves en **CASCADE**; categoría→competidores en **SET NULL**.

4. **No regenerar llaves** (regla del skill). Una categoría con `LlavesGeneradas=true` no se regenera.
   Hoy **no hay "borrar llaves"** → si la clasificación quedó mal, no se puede rehacer. Candidato a historia futura.

5. **Transición de estados del torneo NO existe todavía.** Todos los torneos quedan en `Borrador`
   (no hay endpoint para pasar a Activo/Finalizado). Por eso los guards de "Finalizado" son **latentes**
   (correctos pero inalcanzables hoy). Generar llaves **no** cambia el estado. Posible historia futura.

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
- **Frontend:** `components/bracket/` (bracketLayout, MatchNode, BracketView, GenerarLlavesButton),
  `hooks/useBracket.ts`, `lib/signalr.ts`, ruta `routes/torneos/$torneoId/categorias/$categoriaId/llaves.tsx`.

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

## Cómo verificar H0005 end-to-end (cuando reinicies la API)

1. Login como Coordinador. Crear/usar un torneo en Borrador.
2. Crear ≥2 **categorías** con rangos (ej. una Combate Masculino 12–14, cinturón Blanco–Verde).
3. Cargar **competidores** (≥2 que encajen en una categoría: mismo sexo, edad/peso/graduación en rango).
4. En la página de Categorías → botón **"Generar llaves"** → toast con resumen.
5. En la card de la categoría → **"Ver llaves →"** → se ve el bracket (React Flow, pan/zoom).
6. Clic en un competidor de un match → se marca **ganador** y avanza de ronda (y otros clientes lo ven en vivo).

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

- **Borrar/re-generar llaves** para poder re-clasificar (hoy no existe).
- **Transición de estados** del torneo (Borrador→Activo→Finalizado) — activaría los guards latentes.
- **Multi-categoría** por competidor (Combate + Formas).
- Al eliminar un **competidor que ya está en un match**: la FK de `llaves_competencia` es `Restrict` → daría error.
  Falta guard (bloquear borrado si está en un bracket). Latente hasta que se generen llaves con datos reales.
- Validación de encaje del competidor en su categoría también en edición (hoy la clasificación es solo al generar).
- Nombres de propiedad en errores de validación de rangos aparecen como `RangoEdadMin.Value` (cosmético).
