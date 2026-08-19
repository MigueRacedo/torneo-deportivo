import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';
import type { CreateCategoriaInput } from '@/types';

// Query keys centralizadas por torneo (evita typos y facilita la invalidación).
export const categoriaKeys = {
  all: ['categorias'] as const,
  byTorneo: (torneoId: string) => ['categorias', torneoId] as const,
};

/** Lista las categorías de un torneo (server state en TanStack Query). */
export function useCategorias(torneoId: string) {
  return useQuery({
    queryKey: categoriaKeys.byTorneo(torneoId),
    queryFn: () => api.categorias.listar(torneoId),
    enabled: Boolean(torneoId),
  });
}

/** Crea una categoría en el torneo e invalida la lista al terminar. */
export function useCrearCategoria(torneoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateCategoriaInput) => api.categorias.crear(torneoId, data),
    onSuccess: () => qc.invalidateQueries({ queryKey: categoriaKeys.byTorneo(torneoId) }),
  });
}

/** Edita una categoría del torneo e invalida la lista al terminar. */
export function useEditarCategoria(torneoId: string, categoriaId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateCategoriaInput) => api.categorias.editar(torneoId, categoriaId, data),
    onSuccess: () => qc.invalidateQueries({ queryKey: categoriaKeys.byTorneo(torneoId) }),
  });
}

/** Elimina una categoría del torneo e invalida la lista al terminar. */
export function useEliminarCategoria(torneoId: string) {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (categoriaId: string) => api.categorias.eliminar(torneoId, categoriaId),
    onSuccess: () => qc.invalidateQueries({ queryKey: categoriaKeys.byTorneo(torneoId) }),
  });
}
