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
