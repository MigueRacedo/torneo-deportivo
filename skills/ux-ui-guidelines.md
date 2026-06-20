# Skill: Lineamientos de UX/UI y Accesibilidad (WCAG 2.2 AA)

> Basado en `requerimientos de UX-UI.txt`. Leer **antes** de crear o modificar
> cualquier componente visual del frontend (`frontend/src/components/`,
> `frontend/src/routes/`). Estas reglas son obligatorias, no sugerencias.

## 1. Sistema de Color — Regla 60-30-10

La paleta del proyecto es rojo / azul / blanco. Se reparte así:

| Color | Rol | % aprox. | Uso |
|-------|-----|----------|-----|
| **Blanco / neutro** | Base | 60% | Fondo de la interfaz, cards, contenedores. Prioriza espacio en blanco (no "horror vacui"). |
| **Azul** | Secundario | 30% | Navbar, sidebar, headers de sección, contenedores que necesitan contraste sin competir con el CTA. |
| **Rojo** | Acento | 10% | SOLO para CTAs y acciones principales: "Avanzar de ronda", "Declarar ganador", "Generar llaves", notificaciones críticas. Si el rojo aparece en más del 10% de la pantalla, está mal usado. |

Configurar como tokens en `tailwind.config.ts` (no hardcodear hex en componentes):
```ts
colors: {
  brand: {
    blue: '#...',   // secundario — navbar, headers
    red: '#...',    // acento — CTAs únicamente
  }
}
```

**Reglas de accesibilidad de color:**
- Contraste texto/fondo ≥ **4.5:1** (verificar con herramienta de contraste, no a ojo).
- **Nunca** comunicar estado solo con color. Ej: un competidor descalificado debe tener
  el rojo + un texto ("Descalificado") o un ícono — no solo un borde rojo.
- Esto aplica directamente a `MatchNode` (bracket): el ganador no se marca solo en
  verde, también lleva el ✓ (ya implementado en `skills/react-flow-bracket.md`).

## 2. Tipografía

- Fuente **Sans Serif** (la familia por defecto de Tailwind/shadcn ya cumple).
- Tamaño mínimo: **12px** (`text-xs` de Tailwind = 12px es el piso, no usar nada menor).
- Interlineado mínimo: **1.5** (`leading-relaxed` o superior en bloques de texto).
- Alineación: texto largo **a la izquierda**. Nunca `text-justify`.
- Jerarquía semántica estricta: `<h1>` → `<h2>` → `<h3>`, sin saltar niveles.
  Una página con un `<h1>` no puede tener un `<h3>` sin un `<h2>` intermedio.

## 3. Layout y Grillas

- **Sistema de espaciado de 8pt**: todo margin/padding/gap en múltiplos de 8
  (Tailwind: `p-2`=8px, `p-4`=16px, `p-6`=24px, `p-8`=32px...). Evitar `p-1`, `p-3`,
  `p-5`, `p-7` salvo ajuste fino justificado.
- **Grillas responsive:**
  - Desktop: 12 columnas (`grid-cols-12`)
  - Tablet: 6–8 columnas (`md:grid-cols-6` u `8`)
  - Mobile: 1–4 columnas (`grid-cols-1` a `grid-cols-4`)
- **Ley de Proximidad** en el bracket: los dos competidores de un match deben estar
  visualmente agrupados (mismo contenedor, borde compartido) y separados con espacio
  en blanco de la siguiente ronda — ya es el patrón de `MatchNode` en
  `skills/react-flow-bracket.md` (`divide-y` agrupa el match, `COLUMN_WIDTH` separa rondas).

## 4. Interacción — Ley de Fitts

- **Tamaño mínimo de áreas interactivas:** 24×24px como piso absoluto, **44×44px**
  como estándar para botones de jugadores/llaves y cualquier control táctil
  (Tailwind: `min-h-11 min-w-11` ≈ 44px, o `h-11 w-11`).
- **Foco visible obligatorio:** todo elemento interactivo necesita un anillo de foco
  claro al navegar con `Tab`.
  - **Prohibido** `outline: none` o `focus:outline-none` sin un reemplazo visible.
  - Patrón correcto con Tailwind: `focus-visible:outline focus-visible:outline-3 focus-visible:outline-brand-blue`
    (o `focus-visible:ring-2 focus-visible:ring-offset-2`).

## 5. Prevención de Errores y Formularios

- **Deshacer/Cancelar:** cualquier acción irreversible o fácil de errar (avanzar un
  competidor de ronda, declarar ganador, eliminar inscripción) necesita una vía de
  deshacer (toast con botón "Deshacer" tras la mutación) o una confirmación previa
  (dialog) — no ejecutar directo sin red de seguridad.
- **Labels explícitos:** todo `<input>`/`<select>` en formularios (React Hook Form)
  lleva su `<Label>` visible asociado (`htmlFor`/`id`). El `placeholder` es un
  complemento, nunca el reemplazo del label.
- **Mensajes de error constructivos:** nunca "Se produjo un error". El mensaje debe
  decir qué pasó y cómo resolverlo, ej: "La fecha del torneo no puede ser anterior a
  hoy. Elegí una fecha futura." Esto aplica tanto a errores de Zod/RHF como a los que
  vienen del backend (`ApiError.message` en `lib/api.ts` debe ser específico, no genérico).

## 6. HTML Semántico (base de la accesibilidad)

Antes de cualquier ARIA o CSS complejo, la estructura debe usar las etiquetas
semánticas correctas:
- `<header>` para el encabezado de página/sección.
- `<nav>` para `Navbar`/`Sidebar`.
- `<main>` para el contenido principal de cada ruta (uno por página).
- `<aside>` para contenido complementario (ej. panel de filtros).
- Listas (`<ul>`/`<li>`) para listados de torneos/categorías/competidores, no `<div>` repetidos.

## Checklist rápida (usar antes de dar por terminado un componente)

```
[ ] Blanco domina el fondo; azul en navegación/headers; rojo SOLO en CTAs (≈10%)
[ ] Ningún estado se comunica solo con color (hay texto o ícono de respaldo)
[ ] Contraste texto/fondo ≥ 4.5:1
[ ] Texto ≥ 12px, interlineado ≥ 1.5, alineado a la izquierda (nunca justificado)
[ ] Jerarquía de headings sin saltos (h1→h2→h3)
[ ] Spacing en múltiplos de 8pt
[ ] Grid: 12 col desktop / 6-8 col tablet / 1-4 col mobile
[ ] Targets interactivos ≥ 44x44px (mínimo absoluto 24x24px)
[ ] focus-visible con anillo/outline visible, nunca outline:none sin reemplazo
[ ] Acciones irreversibles tienen Undo o confirmación
[ ] Inputs con <Label> visible, no solo placeholder
[ ] Errores con mensaje específico y accionable
[ ] HTML semántico: header/nav/main/aside antes de pensar en ARIA
```
