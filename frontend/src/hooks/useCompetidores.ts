import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { CreateCompetidorInput, UpdateCompetidorInput } from '@/types';

// Query keys centralizadas por torneo.
export const competidorKeys = {
  all: ['competidores'] as const,
  byTorneo: (torneoId: string) => ['competidores', torneoId] as const,
};

/** Lista los competidores de un torneo (server state en TanStack Query). */
export function useCompetidores(torneoId: string) {
  return useQuery({
    queryKey: competidorKeys.byTorneo(torneoId),
    queryFn: () => api.competidores.listar(torneoId),
    enabled: Boolean(torneoId),
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
