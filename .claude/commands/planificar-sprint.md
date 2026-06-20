# Planificación de Sprint

Genera el plan detallado de tareas para un sprint específico del proyecto.

**Uso:** `/planificar-sprint 1` (para el Sprint 1)

## Releases y Sprints según el Proyecto

### Release 1 — Fundamentos (3 sprints)
**Sprint 1 (2 semanas):**
- H0001: Crear torneo
- H0002: Definir categorías
- Setup inicial del proyecto (infraestructura, Docker, CI/CD básico)
- Autenticación JWT básica (login/logout)

**Sprint 2 (2 semanas):**
- H0004: Cargar competidores
- H0003: Editar torneo
- H0006: Ver categorías (Profesor)

**Sprint 3 (2 semanas):**
- H0005: ⭐ Generar llaves de competencia (el core del proyecto)
- Tests de integración del bracket
- QA del Release 1

### Release 2 — Roles y Consultas (2 sprints)
**Sprint 4 (2 semanas):**
- H0007: Ver competidores (Profesor)
- Panel de Director (métricas básicas)
- Filtros y búsqueda en listas

**Sprint 5 (2 semanas):**
- H0008: Generar reporte PDF
- Registro de resultados (match a match)
- Refinamiento de UX del bracket

### Release 3 — Cierre (1 sprint)
**Sprint 6 (2 semanas):**
- Testing integral (QA Sr)
- Corrección de bugs
- Manual de usuario
- Deploy a producción (piloto en escuela de Taekwondo)
- Documentación técnica

## Formato de Salida al Ejecutar el Comando

Para `/planificar-sprint {N}`, generar:
1. Objetivo del sprint
2. Historias de usuario incluidas con sus criterios de aceptación
3. Desglose de tareas técnicas (con estimación en horas)
4. Dependencias entre tareas
5. Definición de Done (DoD) para el sprint
6. Riesgos específicos de este sprint

Antes de escribir el plan, revisar el estado real del repo (qué historias/endpoints/
componentes ya existen) en vez de asumir que todo arranca de cero — el plan debe
reflejar lo que falta de verdad, no repetir lo que ya está hecho.

## Persistencia del Plan (obligatorio)

Este plan es un documento vivo: se vuelve a generar y actualizar cada vez que se
ejecuta el comando para el mismo sprint, no se descarta después de mostrarlo en el chat.

Pasos, en este orden, en cada ejecución de `/planificar-sprint {N}`:

1. Escribir (o sobrescribir) el contenido completo del plan en
   `docs/sprints/sprint-{N}.md`, usando exclusivamente este subconjunto de Markdown
   (es lo que el conversor de abajo sabe interpretar):
   - `# `, `## `, `### ` para títulos/secciones/subsecciones
   - `- ` para bullets (con o sin `[ ]`/`[x]` de checklist)
   - `1. ` para items numerados
   - tablas con `| col | col |` + fila separadora `|---|---|`
   - texto entre `**` para negrita
   - bloques ```` ``` ```` para diagramas/texto monoespaciado (ej. el árbol de dependencias)
   - `> ` para notas/citas
   - `---` (sola en una línea) para una línea horizontal
2. Generar/actualizar el PDF a partir de ese markdown corriendo, desde la raíz del repo:
   ```bash
   dotnet run --project tools/SprintPdfGenerator -- docs/sprints/sprint-{N}.md docs/sprints/sprint-{N}.pdf
   ```
3. Confirmar que el comando anterior terminó con "PDF generado en: ..." antes de dar
   la tarea por terminada — si falla, arreglar el Markdown o el generador, no ignorar el error.
4. Avisar al usuario la ruta de los dos archivos (`.md` fuente y `.pdf` generado) en la respuesta.

El `.md` es la fuente editable (en este archivo o a mano) que se va actualizando sprint
a sprint; el `.pdf` es el artefacto de consulta que se regenera siempre desde el `.md`,
nunca se edita el PDF directamente.
