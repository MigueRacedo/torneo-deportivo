import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect } from 'react';
import { categoriaKeys } from '@/hooks/useCategorias';
import { competidorKeys } from '@/hooks/useCompetidores';
import { api } from '@/lib/api';
import { crearConexionBracket } from '@/lib/signalr';

// Query keys centralizadas del bracket. `byTorneo` es prefijo de `detail`, así que
// invalidarla alcanza a los brackets de todas las categorías del torneo.
export const bracketKeys = {
  all: ['bracket'] as const,
  byTorneo: (torneoId: string) => ['bracket', torneoId] as const,
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

/**
 * Suscribe la página a las actualizaciones en vivo del bracket (SignalR) y refresca los brackets del torneo
 * cuando otro cliente registra un ganador.
 *
 * La conexión se abre una sola vez por torneo y la invalidación usa `byTorneo`, no `detail`: el evento
 * `MatchActualizado` es de alcance torneo, así que cambiar de categoría en la vista no debe reabrir el socket
 * ni dejar desactualizados los brackets que no se están mirando.
 */
export function useBracketLiveUpdates(torneoId: string) {
  const qc = useQueryClient();

  useEffect(() => {
    if (!torneoId) return;
    const connection = crearConexionBracket(torneoId);
    connection.on('MatchActualizado', () => {
      qc.invalidateQueries({ queryKey: bracketKeys.byTorneo(torneoId) });
    });
    return () => {
      connection.off('MatchActualizado');
      void connection.stop();
    };
  }, [torneoId, qc]);
}

/** Genera las llaves del torneo (clasifica + arma brackets) e invalida categorías y competidores. */
export function useGenerarLlaves(torneoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: () => api.llaves.generar(torneoId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: categoriaKeys.byTorneo(torneoId) });
      qc.invalidateQueries({ queryKey: competidorKeys.byTorneo(torneoId) });
      qc.invalidateQueries({ queryKey: bracketKeys.byTorneo(torneoId) });
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
