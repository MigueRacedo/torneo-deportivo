import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { CreateCompetidorInput, UpdateCompetidorInput } from '@/types';

// Query keys centralizadas por torneo.
export const competidorKeys = {
  all: ['competidores'] as const,
  byTorneo: (torneoId: string) => ['competidores', torneoId] as const,
  // Es hijo de byTorneo: invalidar el listado del torneo tras una mutación también refresca esta vista.
  misAlumnos: (torneoId: string) => ['competidores', torneoId, 'mis-alumnos'] as const,
};

/**
 * Alumnos del Profesor autenticado en un torneo (H0007).
 *
 * A diferencia de `useCompetidores`, este endpoint SÍ está abierto al Profesor: devuelve únicamente los
 * competidores de su propia escuela, resuelta server-side desde el token.
 */
export function useMisCompetidores(torneoId: string) {
  return useQuery({
    queryKey: competidorKeys.misAlumnos(torneoId),
    queryFn: () => api.competidores.misAlumnos(torneoId),
    enabled: Boolean(torneoId),
  });
}

/**
 * Lista los competidores de un torneo (server state en TanStack Query).
 *
 * El endpoint es **solo Coordinador**: un Profesor ve únicamente los de su escuela (H0007). Por eso
 * las pantallas que comparte con otros roles deben deshabilitar la consulta cuando el usuario no es
 * Coordinador; si no, responde 403 y rompe toda la vista.
 */
export function useCompetidores(torneoId: string, opciones?: { enabled?: boolean }) {
  return useQuery({
    queryKey: competidorKeys.byTorneo(torneoId),
    queryFn: () => api.competidores.listar(torneoId),
    enabled: Boolean(torneoId) && (opciones?.enabled ?? true),
  });
}

/** Carga un competidor en el torneo e invalida la lista al terminar. */
export function useCargarCompetidor(torneoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateCompetidorInput) => api.competidores.cargar(torneoId, data),
    onSuccess: () => qc.invalidateQueries({ queryKey: competidorKeys.byTorneo(torneoId) }),
  });
}

/** Edita un competidor del torneo e invalida la lista al terminar. */
export function useEditarCompetidor(torneoId: string, competidorId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: UpdateCompetidorInput) => api.competidores.editar(torneoId, competidorId, data),
    onSuccess: () => qc.invalidateQueries({ queryKey: competidorKeys.byTorneo(torneoId) }),
  });
}

/** Elimina un competidor del torneo e invalida la lista al terminar. */
export function useEliminarCompetidor(torneoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (competidorId: string) => api.competidores.eliminar(torneoId, competidorId),
    onSuccess: () => qc.invalidateQueries({ queryKey: competidorKeys.byTorneo(torneoId) }),
  });
}
