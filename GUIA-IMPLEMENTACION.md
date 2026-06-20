# Guía Completa: Implementar el Proyecto con Claude

## Proyecto: Sistema de Gestión de Torneos Deportivos de Contacto
**Autor del documento de proyecto:** Miguel Racedo | UTN FRBA K4051 | 2025
**Stack:** .NET 10 + React 19 + Vite (versión 2026, proyecto personal)

---

## ¿Qué son los archivos .md para Claude?

Claude Code (la herramienta de terminal de Anthropic) tiene un sistema de **contexto persistente** basado en archivos Markdown. Estos archivos se leen automáticamente cuando abrís Claude Code y le dan "memoria" sobre tu proyecto sin que tengas que explicar el contexto cada vez.

### Los 4 tipos de archivos que necesitás

```
1. CLAUDE.md (raíz)        → El cerebro del proyecto
2. {carpeta}/CLAUDE.md     → Contexto específico de cada módulo
3. .claude/commands/*.md   → Comandos reutilizables (slash commands)
4. skills/*.md             → Guías especializadas para tareas complejas
```

---

## El Stack y Por Qué (versión aprendizaje)

Como es un proyecto personal y querés aprender, este stack te expone a las
herramientas modernas que se usan hoy en la industria:

### Frontend
| Tecnología       | Qué aprendés                                         |
|------------------|------------------------------------------------------|
| React 19 + Vite  | SPA moderna, build rápido, lo más usado en frontend  |
| TanStack Query   | Manejo de server state (cache, refetch, sync) — skill muy demandada |
| Zustand          | State management ligero para client state            |
| React Hook Form + Zod | Formularios performantes + validación tipada    |
| shadcn/ui + Tailwind | Sistema de componentes moderno                   |
| React Flow       | Visualización de grafos/árboles (el bracket)         |
| SignalR (cliente)| Tiempo real (WebSockets)                             |

### Backend
| Tecnología       | Qué aprendés                                         |
|------------------|------------------------------------------------------|
| .NET 10 + C# 14  | Última versión, lo que ya estás usando               |
| FastEndpoints    | APIs REST minimalistas (REPR pattern)                |
| Clean Architecture | Separación de capas Domain/Application/Infra/API   |
| CQRS + MediatR   | Separar lecturas de escrituras                       |
| FluentValidation | Validación declarativa                               |
| Mapster          | Mapeo objeto-a-objeto source-generated               |
| EF Core 10       | ORM, migraciones, LINQ                               |
| QuestPDF         | Generación de PDFs con API fluent                    |
| SignalR (server) | Hubs de tiempo real                                  |

### Infraestructura
| Tecnología       | Qué aprendés                                         |
|------------------|------------------------------------------------------|
| PostgreSQL 16    | Base de datos relacional robusta                     |
| Redis            | Cache en memoria                                      |
| MinIO / S3       | Object storage (imágenes)                            |
| Docker Compose   | Orquestación local de servicios                      |
| GitHub Actions   | CI/CD automático                                     |
| Fly.io / Railway | Deploy moderno y simple                              |

---

## Estructura de Archivos del Proyecto

```
torneo-deportivo/
│
├── 📋 CLAUDE.md                    ← Lee esto primero
│   (Stack, arquitectura, entidades, convenciones globales)
│
├── 📋 backend/CLAUDE.md            ← Contexto del backend (.NET 10 + FastEndpoints)
│
├── 📋 frontend/CLAUDE.md           ← Contexto del frontend (React + TanStack Query)
│
├── 🛠️ skills/
│   ├── bracket-algorithm.md        ← Algoritmo de llaves (lógica de datos)
│   ├── react-flow-bracket.md       ← Visualización del bracket con React Flow
│   └── database-schema.md          ← Schema SQL completo
│
├── ⚡ .claude/commands/
│   ├── implementar-historia.md     → /implementar-historia H0001
│   ├── code-review.md              → /code-review [backend|frontend]
│   └── planificar-sprint.md        → /planificar-sprint 1
│
├── 🐳 docker-compose.yml           (PostgreSQL + Redis + MinIO + backend + frontend)
├── 📦 backend/                     ← .NET 10 Web API
└── 🌐 frontend/                    ← React 19 + Vite
```

---

## Cómo Funciona Cada Archivo

### 1. `CLAUDE.md` (Raíz) — El Cerebro
Claude Code lo lee **automáticamente** al iniciar en un directorio.
**Regla de oro:** Si tenés que explicarle algo a Claude más de una vez, va al `CLAUDE.md`.

### 2. `{módulo}/CLAUDE.md` — Contexto por Módulo
Claude los lee cuando navegás a esa carpeta o los mencionás. Más detallados que el raíz.

### 3. `.claude/commands/*.md` — Slash Commands
Comandos que ejecutás escribiendo `/nombre-del-comando` en Claude Code.
```bash
/implementar-historia H0005    # Implementa la historia de generar llaves
/code-review backend           # Revisa el código del backend
/planificar-sprint 3           # Genera el plan del Sprint 3
```

### 4. `skills/*.md` — Skills Especializadas
Guías que Claude lee cuando necesita conocimiento especializado. No se leen
automáticamente — Claude los lee cuando se lo pedís o cuando el CLAUDE.md lo indica.

---

## Flujo de Trabajo con Claude Code

### Instalación
```bash
npm install -g @anthropic-ai/claude-code
```

### Uso Diario
```bash
cd torneo-deportivo/
claude                          # lee automáticamente CLAUDE.md

# Claude ya conoce tu proyecto:
> Implementa el endpoint POST /api/v1/torneos con FastEndpoints

# Para tareas específicas, decirle qué skills leer:
> Lee skills/bracket-algorithm.md y skills/react-flow-bracket.md, luego implementa H0005

# Slash commands:
> /implementar-historia H0001
```

---

## Guía Paso a Paso: Construir el Proyecto Desde Cero

### Fase 0: Setup Inicial
```bash
mkdir torneo-deportivo && cd torneo-deportivo
mkdir -p .claude/commands skills
git init
echo "node_modules/\n.env*\nbin/\nobj/\ndist/" > .gitignore
```

Con Claude Code:
```
> Crea el docker-compose.yml con PostgreSQL 16, Redis, MinIO, backend .NET y 
> frontend Vite, todos en una red compartida
```

### Fase 1: Backend (.NET 10)
```
> Lee backend/CLAUDE.md y crea la solución .NET 10 con Clean Architecture (4 proyectos).
> Instala: FastEndpoints, MediatR, FluentValidation, Mapster, 
> Microsoft.EntityFrameworkCore, Npgsql.EntityFrameworkCore.PostgreSQL, 
> EFCore.NamingConventions, QuestPDF, Minio
```

Orden sugerido:
```
1. Domain: Entidades Torneo, Categoria, Competidor, Match
2. Infrastructure: TorneoDbContext + configuraciones EF
3. Infrastructure: Primera migración
4. Application: CreateTorneo Command + Handler
5. API: CreateTorneoEndpoint (FastEndpoints) + Validator
6. Tests: CreateTorneoCommandHandlerTests
```

### Fase 2: Frontend (React + Vite)
```
> Lee frontend/CLAUDE.md y crea el proyecto con: 
> npm create vite@latest frontend -- --template react-ts
> Luego instala: @tanstack/react-query, zustand, react-hook-form, zod, 
> @xyflow/react, @microsoft/signalr, tailwindcss
> y configura shadcn/ui
```

Orden:
```
1. Configurar TanStack Query (QueryClientProvider en main.tsx)
2. Layout base: Navbar + Sidebar
3. Autenticación: login + Zustand authStore + JWT
4. Tipos TypeScript (types/index.ts)
5. API client (lib/api.ts)
6. Primer hook + página: useTorneos + lista de torneos
7. Bracket con React Flow (dejar para H0005)
```

### Fase 3: Funcionalidades por Historia
```
/implementar-historia H0001  ← Crear torneo
/implementar-historia H0002  ← Definir categorías
/implementar-historia H0004  ← Cargar competidores
/implementar-historia H0005  ← Generar llaves (CORE — el más complejo)
```

---

## Tips para Usar Claude Eficientemente

### ✅ Hacer
- **Dar contexto específico:** "Lee skills/react-flow-bracket.md y luego..."
- **Pedir por partes:** primero el handler, luego el endpoint, luego el frontend
- **Revisar antes de continuar:** `/code-review` antes de la siguiente historia
- **Actualizar los CLAUDE.md** cuando tomes decisiones de arquitectura

### ❌ Evitar
- Pedirle que "haga todo el backend" en una sola instrucción
- No leer los CLAUDE.md antes de modificar código existente
- Ignorar errores de compilación antes de avanzar

### 🔄 Iteración
```
Pedir → Revisar → Ajustar → Commit → Repetir
```

---

## Ejemplo Real: Implementar H0005 (Generar Llaves)

La historia más compleja (13 story points). Tiene dos partes:
**el algoritmo (backend)** y **la visualización (frontend con React Flow)**.

**Paso 1:** Verificar el algoritmo
```
> Abrí skills/bracket-algorithm.md y verificá que la distribución de BYEs 
> sea correcta para 6 competidores
```

**Paso 2:** Backend — el servicio
```
> Lee backend/CLAUDE.md y skills/bracket-algorithm.md. 
> Crea BracketGeneratorService en Application/Services/ con el algoritmo
```

**Paso 3:** Tests primero (TDD)
```
> Crea los tests unitarios para BracketGeneratorService cubriendo todos 
> los casos de "Tests Obligatorios" del skill
```

**Paso 4:** Backend — el endpoint + SignalR
```
> Implementa GenerarLlavesCommand, el endpoint POST .../llaves/generar, 
> y el BracketHub de SignalR para emitir MatchActualizado
```

**Paso 5:** Frontend — React Flow
```
> Lee frontend/CLAUDE.md y skills/react-flow-bracket.md. 
> Implementa bracketLayout.ts, BracketView.tsx y MatchNode.tsx con React Flow.
> Conectá SignalR para refrescar el bracket cuando se registra un ganador.
```

---

## Preguntas Frecuentes

**¿Qué pasa si Claude olvida el contexto?**
→ Claude Code re-lee los CLAUDE.md. Si olvidó algo: "Lee el CLAUDE.md raíz y 
  backend/CLAUDE.md antes de responder"

**¿Cuándo actualizo los CLAUDE.md?**
→ Cada vez que tomás una decisión de arquitectura que Claude necesitará recordar.

**¿Vite o Next.js?**
→ Para este proyecto, Vite. Es un dashboard privado sin SEO. Si más adelante querés
  una vista pública del bracket para espectadores, ahí evaluás Next.js.

**¿FastEndpoints o Controllers?**
→ FastEndpoints. Menos código, mejor performance, encaja con CQRS. Si venís de
  Controllers, la curva es de un par de horas.

**¿Por qué TanStack Query en vez de hooks manuales?**
→ Te ahorra escribir loading/error/cache/refetch a mano en cada hook. Es el estándar
  de facto para data fetching en React hoy.

**¿Qué es Claude Code vs claude.ai?**
→ Claude Code trabaja directamente con tus archivos locales (lee, escribe, ejecuta).
  claude.ai es la interfaz web. Para desarrollar, Claude Code es mucho más poderoso.
