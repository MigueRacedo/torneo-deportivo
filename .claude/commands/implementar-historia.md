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

3. **Implementar BACKEND en este orden:**
   a. **Entidad de Dominio** (si es nueva) en `Domain/Entities/`
   b. **Configuración EF** en `Infrastructure/Persistence/Configurations/`
   c. **Migración** (generar el comando, no ejecutar)
   d. **Command o Query + Handler** en `Application/Features/`
   e. **Response/Request records** y config de Mapster si hace falta
   f. **Endpoint (FastEndpoints)** en `API/Endpoints/` con su Validator
      - Recordar `Roles("...")` según la tabla de roles del CLAUDE.md raíz
   g. **Tests** del handler (xUnit + NSubstitute)

4. **Implementar FRONTEND en este orden:**
   h. **Types TypeScript** (si hay nuevos) en `src/types/index.ts`
   i. **Método del API client** en `src/lib/api.ts`
   j. **Hook de TanStack Query** en `src/hooks/` (con query keys centralizadas)
   k. **Componentes de UI** en `src/components/` (con React Hook Form + Zod si hay form)
   l. **Ruta/página** en `src/routes/`

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
