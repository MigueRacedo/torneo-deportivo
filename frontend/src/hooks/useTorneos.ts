import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export const torneoKeys = {
  all: ['torneos'] as const,
  detail: (id: string) => ['torneos', id] as const,
};

export function useTorneos() {
  return useQuery({
    queryKey: torneoKeys.all,
    queryFn: api.torneos.listar,
  });
}

export function useTorneo(id: string) {
  return useQuery({
    queryKey: torneoKeys.detail(id),
    queryFn: () => api.torneos.obtener(id),
    enabled: Boolean(id),
  });
}

export function useCrearTorneo() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: api.torneos.crear,
    onSuccess: () => qc.invalidateQueries({ queryKey: torneoKeys.all }),
  });
}
