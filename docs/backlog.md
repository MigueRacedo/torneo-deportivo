# Backlog detallado y deuda técnica

> Especificaciones de las historias pendientes y deuda anotada. Se lee **antes de implementar** una
> historia, no en cada sesión — por eso vive acá y no en `CLAUDE.md`.
> La tabla resumen de historias y el modelo de estados están en `CLAUDE.md` (raíz).

## Historias pendientes

| ID | Título | Épica | Puntos | Rol |
|----|--------|-------|--------|-----|

| H0008 | Generar reporte PDF | Reportes Director | 8 | Director |
| H0009 | **Ciclo de vida del torneo** | Gestión del torneo | 8 | Coordinador |
| H0010 | Rehacer llaves de una categoría | Inscripción y llaves | 5 | Coordinador |
| H0011 | Doble participación (Combate + Formas) | Inscripción y llaves | 13 | Coordinador |
| H0012 | Categoría desierta | Inscripción y llaves | 3 | Coordinador |
| H0013 | Tablas de competidores filtrables/ordenables/paginadas | Usabilidad | 5 | Coordinador + Profesor |

- H0008: usar **QuestPDF** (ya está la licencia Community configurada en `Infrastructure/DependencyInjection.cs`).

### Orden de implementación

**H0009 → H0010 → H0012 → H0014 → H0015 → H0016 → H0008 → H0011 → H0013.**

El razonamiento completo (dependencias, costo de retrabajo y los puntos discutibles) está en `CLAUDE.md`
(raíz), sección **"Orden recomendado de implementación"**. Acá abajo va el detalle de cada historia.

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

### H0013 — Tablas de competidores filtrables, ordenables y paginadas · 5 pts · Performance

**Origen:** pedido del usuario (2026-08-20). Con 15 competidores ya cuesta encontrar uno; un torneo real
tiene cientos y hoy las tablas son listados planos sin ningún control.

**Alcance — las dos tablas de competidores que existen hoy:**
- `components/competidores/CompetidorList.tsx` (página de competidores del torneo).
- `components/bracket/CompetidoresSinCategoria.tsx` (vista consolidada de llaves).
- **H0007 ya creó una tercera tabla** (`components/competidores/MisAlumnosList.tsx`): al hacer H0013 hay que migrar las tres al componente compartido.

**Implementación:** extraer un componente de tabla reutilizable (ej. `components/ui/data-table.tsx`) con
**`@tanstack/react-table`** — hay que agregar la dependencia, hoy no está instalada. Es la opción coherente
con el stack (ya se usa TanStack Query) y es headless, así que no pelea con Tailwind ni con shadcn/ui.

**Decisión de diseño clave — paginación del lado del CLIENTE, no del servidor:**
Es tentador paginar en el backend, pero hoy sería un error. Varias vistas ya **derivan datos del listado
completo**: `routes/torneos/$torneoId/llaves.tsx` calcula `competidoresPorCategoria` recorriendo todos los
competidores, y `sinCategoria` filtra sobre el total. Con paginación server-side esos conteos pasarían a
ser "los de la página actual", que es simplemente incorrecto. Sumado a que un torneo son cientos de filas
(no miles), la paginación en cliente sobre la query que ya está en cache es lo correcto.
→ Migrar a server-side recién si un torneo supera ~2000 competidores, y entonces habrá que agregar
endpoints de conteo agregado para las vistas derivadas.

**Criterios de aceptación:**
- Filtro de texto libre (nombre/apellido/escuela) y filtros por columna en sexo, graduación y categoría.
- Orden ascendente/descendente por columna, con indicador visible del criterio activo.
- Paginación con tamaño de página configurable y total de resultados a la vista.
- El estado de filtro/orden/página es **UI state**, no server state: no va a TanStack Query (regla 7 de
  `CLAUDE.md` raíz). Evaluar reflejarlo en la URL como se hizo con `?categoria=` en la vista de llaves.

**Accesibilidad (obligatorio, `skills/ux-ui-guidelines.md`):**
- Los headers ordenables son `<button>` dentro del `<th>`, con **`aria-sort`** en el `th`
  (`ascending`/`descending`/`none`) — el orden no puede comunicarse solo con un ícono.
- Los controles de paginación necesitan nombre accesible ("Página siguiente", no solo "›") y ≥44×44px.
- Los inputs de filtro llevan `<Label>` visible, no solo placeholder.
- Anunciar el resultado del filtrado en una región `aria-live` ("12 de 150 competidores").

**Atadura con H0011:** la columna y el filtro de "Categoría" se rehacen cuando un competidor pase a tener
varias (Combate + Formas). El resto de la tabla no se ve afectado.

---

---

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

## Deuda técnica anotada

> La deuda de H0007 —el filtro por texto y la falta de UI para asignar escuela a un Profesor— quedó
> promovida a historias: **H0014** y **H0015**. Ver §"Bloque de datos maestros".

> Los tres follow-ups grandes que estaban acá (borrar/regenerar llaves, transición de estados y
> multi-categoría) **se promovieron a historias**: H0010, H0009 y H0011 respectivamente (ver arriba).

Queda como deuda suelta:

- Al eliminar un **competidor que ya está en un match**: la FK de `llaves_competencia` es `Restrict` → daría error.
  Falta guard aplicativo (hoy sale como error crudo de EF en vez de un 409 con mensaje claro).
  **Ya no es latente**: con H0005 verificado E2E hay llaves con datos reales, así que es reproducible.
- Validación de encaje del competidor en su categoría también en edición (hoy la clasificación es solo al generar).
- Nombres de propiedad en errores de validación de rangos aparecen como `RangoEdadMin.Value` (cosmético).
- `frontend`: 1 warning de lint preexistente en `CategoriaForm` (`watch()` de React Hook Form es
  incompatible con el React Compiler y saltea la memoización del componente).

---

## Bloque de datos maestros: H0014 → H0015 → H0016

**Origen:** observación del usuario (2026-08-26) — *"no me parece bueno que se tenga que comparar texto
con texto; escuela y responsable podrían ser un combo desplegable"*.

**El problema.** `Competidor.Escuela`, `Competidor.Responsable` y `Usuario.Escuela` son **texto libre**.
El filtro de H0007 compara `usuario.Escuela == competidor.Escuela` por igualdad exacta: un espacio de
más, una mayúscula distinta o "Esc. Central" vs "Escuela Central" hacen que el Profesor **no vea a sus
alumnos, sin ningún error visible**. Lo mismo va a romper los agrupamientos por escuela de H0008.

**Por qué conviene no postergarlo.** El costo del backfill crece con cada torneo cargado: hoy son pocas
variantes de texto por normalizar, dentro de unos meses es limpieza de datos a mano.

### H0014 — Catálogo de escuelas · 8 pts · Básico

- Entidad `Escuela` (`id`, `nombre` único, `activa`) + CRUD mínimo para el Coordinador.
- `Competidor.EscuelaId` y `Usuario.EscuelaId` como **FK**, reemplazando los strings.
- **Migración con backfill**, es la parte delicada: crear una fila `escuelas` por cada nombre distinto
  ya existente (normalizando espacios y mayúsculas) y mapear los competidores/usuarios a esos ids antes
  de borrar las columnas de texto. Debe ser reversible.
- Combo desplegable en el alta y edición de competidor, en lugar del `<input>` de texto.
- Al terminar, el filtro de H0007 pasa a ser una comparación de ids: se puede borrar la comparación de
  strings de `GetMisCompetidoresQueryHandler`.
- FK `escuela → competidores`: **RESTRICT** (no se borra una escuela con inscriptos).

### H0015 — Gestión de usuarios (alta de Profesores) · 5 pts · Básico

- CRUD de usuarios para el Coordinador: nombre, email, rol, escuela y contraseña inicial.
- Cierra la deuda anotada en H0007: hoy la **única** forma de crear un Profesor o de asignarle escuela
  es el `DbSeeder`, y el seeder solo corre en Development.
- Es **prerrequisito de H0016**: para elegir un profesor de un combo tiene que haber profesores cargados.
- Ojo con el hash de contraseña: reutilizar `IPasswordHasher`, nunca guardar texto plano (ya cubierto
  por un test del seeder).

### H0016 — Profesor responsable del competidor (FK) · 5 pts · Performance

- `Competidor.ProfesorId` como FK a `Usuario` (rol Profesor), con combo en el alta **filtrado por la
  escuela ya elegida**.
- "Mis alumnos" (H0007) puede pasar a filtrar por **profesor** en vez de por escuela: más preciso cuando
  una escuela tiene varios profesores.
- ⚠️ **Punto a definir antes de implementar:** hoy `responsable` es texto libre y convive con `telefono`,
  sin aclarar si es el profesor o el adulto a cargo. Si `responsable` se convierte en FK al profesor,
  `telefono` queda huérfano (el teléfono del profesor va en su usuario, no repetido en cada competidor)
  y se pierde el contacto del adulto responsable — relevante en un deporte de contacto con menores.
  **Recomendación:** separar en dos campos, `profesorId` (FK) y `contactoEmergencia` + `telefono` (texto).
- Depende de **H0015** (que existan profesores) y **H0014** (para filtrar el combo por escuela).

### Dónde encaja este bloque en el orden general

Es **ortogonal** a H0009–H0012 (ciclo de vida y llaves): toca columnas distintas de `competidores` y no
comparte código con el clasificador ni con el bracket.

En el orden acordado va en las **posiciones 4 a 6**, después de la cadena de llaves y antes de H0008:

- El backfill de H0014 se encarece con cada torneo que se cargue con texto libre.
- H0008 (reportes del Director) necesita agrupar por escuela de forma confiable, así que va después.
- ⚠️ No solaparlo con **H0011**, que también migra `competidores`: cada migración debería poder
  revisarse por separado.

### Deuda que este bloque absorbe (de H0007)

- El filtro de "mis alumnos" compara **escuela por texto exacto**: falla en silencio con diferencias de
  espacios o mayúsculas. → **H0014**.
- **No hay UI para asignar la escuela a un Profesor**: solo la pone el `DbSeeder`, que además corre
  únicamente en Development. → **H0015**.
