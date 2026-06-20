# Revisión de Código

Realiza una code review completa de los cambios recientes del proyecto.

**Uso:** `/code-review` o `/code-review backend` o `/code-review frontend`

## Backend (.NET 10 + FastEndpoints)
**Arquitectura:**
- [ ] Las entidades de negocio solo están en `Domain/`
- [ ] Los casos de uso están en `Application/Features/` (Command/Query/Handler)
- [ ] Los endpoints (FastEndpoints) no tienen lógica de negocio (solo coordinan vía MediatR)
- [ ] Los repositorios están en `Infrastructure/`, no hay queries EF en `Application/`

**Calidad:**
- [ ] Cada endpoint declara `Roles("...")` correcto en Configure()
- [ ] Los Requests tienen su `Validator<T>` correspondiente
- [ ] Los handlers manejan errores con excepciones tipadas (`NotFoundException`, etc.)
- [ ] No hay `null` sin manejo (nullable reference types activado)
- [ ] No hay `async void` (siempre `async Task`)
- [ ] Los métodos asíncronos reciben y propagan `CancellationToken ct`
- [ ] El mapeo usa Mapster (`.Adapt<T>()`), no construcción manual repetitiva

**Tests:**
- [ ] Test para el happy path
- [ ] Test para input inválido (ValidationException)
- [ ] Test para entidad no encontrada (NotFoundException)
- [ ] Si toca el bracket: tests de todos los casos de `skills/bracket-algorithm.md`

## Frontend (React + Vite + TanStack Query)
**Estructura:**
- [ ] Los componentes de dominio están en `components/{entidad}/`
- [ ] Los hooks de datos usan TanStack Query y están en `hooks/`
- [ ] Las query keys están centralizadas (objeto `{entidad}Keys`)
- [ ] Las llamadas a API pasan siempre por `lib/api.ts`
- [ ] Los tipos están centralizados en `types/index.ts`

**Calidad:**
- [ ] El server state vive SOLO en TanStack Query (no duplicado en useState/Zustand)
- [ ] Las mutaciones invalidan las queries relevantes en `onSuccess`
- [ ] Los formularios usan React Hook Form + Zod
- [ ] Las rutas protegidas tienen `RoleGuard`
- [ ] Los componentes son responsive (clases md: y lg: de Tailwind)
- [ ] No hay `any` en TypeScript

**UX:**
- [ ] Los estados de carga (`isLoading`) muestran skeleton o spinner
- [ ] Los errores (`error`) se muestran al usuario, no solo en console
- [ ] Los formularios deshabilitan el submit durante el envío (`isPending`)
- [ ] Los mensajes de éxito tienen feedback visual (toast)
- [ ] Si toca el bracket: pan + zoom funcionan, BYEs y ganadores se ven bien

**Accesibilidad y UX/UI (ver `skills/ux-ui-guidelines.md`):**
- [ ] Color: blanco domina el fondo (60%), azul en navegación/secundarios (30%),
      rojo SOLO en CTAs/acciones principales (10%) — no más del 10% de rojo en pantalla
- [ ] Ningún estado se comunica solo con color (siempre + texto o ícono de respaldo)
- [ ] Contraste de texto/fondo ≥ 4.5:1
- [ ] Texto ≥ 12px, interlineado ≥ 1.5, alineado a la izquierda (nunca `text-justify`)
- [ ] Jerarquía de headings sin saltos (`h1`→`h2`→`h3`, sin pasar de h1 a h3)
- [ ] Spacing (margin/padding/gap) en múltiplos de 8pt
- [ ] Grid responsive: 12 columnas desktop, 6-8 tablet, 1-4 mobile
- [ ] Áreas interactivas ≥ 44x44px (mínimo absoluto 24x24px)
- [ ] `focus-visible` con anillo/outline visible en todo elemento interactivo —
      **prohibido** `outline: none`/`focus:outline-none` sin reemplazo
- [ ] Acciones irreversibles (avanzar ronda, declarar ganador, eliminar) tienen
      Undo o confirmación previa
- [ ] Todo `<input>`/`<select>` tiene `<Label>` visible asociado (no solo placeholder)
- [ ] Mensajes de error específicos y accionables (nunca "Se produjo un error")
- [ ] HTML semántico (`header`, `nav`, `main`, `aside`) antes de ARIA/CSS complejo

## Salida Esperada
1. Resumen de lo revisado
2. Problemas encontrados (severity: 🔴 Critical / 🟡 Warning / 🔵 Info)
3. Sugerencias de mejora
4. Código corregido para los problemas críticos
