import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { categoriaKeys } from '@/hooks/useCategorias';
import { competidorKeys } from '@/hooks/useCompetidores';
import { api } from '@/lib/api';

// Query keys centralizadas del bracket.
export const bracketKeys = {
  detail: (torneoId: string, categoriaId: string) => ['bracket', torneoId, categoriaId] as const,
};

/** Obtiene el bracket de una categoría (server state en TanStack Query). */
export function useBracket(torneoId: string, categoriaId: string) {
  return useQuery({
    queryKey: bracketKeys.detail(torneoId, categoriaId),
    queryFn: () => api.llaves.obtener(torneoId, categoriaId),
    enabled: Boolean(torneoId && categoriaId),
  });
}

/** Genera las llaves del torneo (clasifica + arma brackets) e invalida categorías y competidores. */
export function useGenerarLlaves(torneoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => api.llaves.generar(torneoId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: categoriaKeys.byTorneo(torneoId) });
      qc.invalidateQueries({ queryKey: competidorKeys.byTorneo(torneoId) });
    },
  });
}

/** Registra el ganador de un match e invalida el bracket de la categoría. */
export function useRegistrarGanador(torneoId: string, categoriaId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ llaveId, ganadorId }: { llaveId: string; ganadorId: string }) =>
      api.llaves.registrarGanador(torneoId, llaveId, ganadorId),
    onSuccess: () => qc.invalidateQueries({ queryKey: bracketKeys.detail(torneoId, categoriaId) }),
  });
}
