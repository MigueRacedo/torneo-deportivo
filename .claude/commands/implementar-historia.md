# Implementar Historia de Usuario

Implementa una historia de usuario completa del sistema de torneos.

**Uso:** `/implementar-historia H0001` o `/implementar-historia H0004`

## Instrucciones para Claude

Cuando se ejecute este comando con un ID de historia (ej: H0001):

1. **Buscar la historia** en el CLAUDE.md raíz (tabla de Historias de Usuario)

2. **Leer los skills relevantes** según la historia:
   - Si involucra bracket/llaves → leer `skills/bracket-algorithm.md` Y `skills/react-flow-bracket.md`
   - Siempre leer `backend/CLAUDE.md` y `frontend/CLAUDE.md`
   - Para BD → leer `skills/database-schema.md`
   - Siempre leer `skills/ux-ui-guidelines.md`

3. **Implementar BACKEND en este orden:**
   1. **Entidad de Dominio** (si es nueva) en `Domain/Entities/`
   2. **Configuración EF** en `Infrastructure/Persistence/Configurations/`
   3. **Migración** (generar el comando, no ejecutar)
   4. **Command o Query + Handler** en `Application/Features/`
   5. **Response/Request records** y config de Mapster si hace falta
   6. **Endpoint (FastEndpoints)** en `API/Endpoints/` con su Validator
      - Recordar `Roles("...")` según la tabla de roles del CLAUDE.md raíz
   7. **Tests** del handler (xUnit + NSubstitute)

4. **Implementar FRONTEND en este orden:**
   1. **Types TypeScript** (si hay nuevos) en `src/types/index.ts`
   2. **Método del API client** en `src/lib/api.ts`
   3. **Hook de TanStack Query** en `src/hooks/` (con query keys centralizadas)
   4. **Componentes de UI** en `src/components/` (con React Hook Form + Zod si hay form)
   5. **Ruta/página** en `src/routes/`

5. **Generar un checklist** al final mostrando qué se implementó

6. **Generar un documento pdf** al final explicando de cada decisión y cosas que se implementaron en el proyecto. Claro como para entender casi sin la necesidad de revisar todo el código.

## Reglas específicas del stack
- Backend: usar **FastEndpoints** (clase `Endpoint<Req, Res>`), no Controllers
- Backend: mapeo con **Mapster** (`.Adapt<T>()`), no AutoMapper
- Frontend: server state SIEMPRE en **TanStack Query**, nunca en useState
- Frontend: mutaciones invalidan queries en `onSuccess`
- Si la historia es H0005 (generar llaves): el bracket usa **React Flow**, leer
  ambos skills de bracket antes de empezar

## Ejemplo de uso
```
/implementar-historia H0005
```
→ Claude lee los skills de bracket, implementa BracketGeneratorService + endpoint
  en el backend, y BracketView con React Flow + SignalR en el frontend.

## Notas
- Mantener el mismo estilo del código existente
- Los tests del handler son obligatorios
- El componente de UI debe ser responsive (mobile-first con Tailwind)
