import type { CreateTorneoInput, Torneo } from '@/types';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000';

export class ApiError extends Error {
  status: number;

  constructor(status: number, message: string) {
    super(message);
    this.status = status;
  }
}

async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const token = localStorage.getItem('token');
  const res = await fetch(`${BASE_URL}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options?.headers,
    },
  });

  if (!res.ok) {
    const body = await res.json().catch(() => null);
    const message =
      body?.errors?.[0]?.message ?? body?.message ?? 'No se pudo completar la operación. Intentá de nuevo.';
    throw new ApiError(res.status, message);
  }

  return res.status === 204 ? (undefined as T) : res.json();
}

export const api = {
  torneos: {
    listar: () => apiFetch<Torneo[]>('/api/v1/torneos'),
    obtener: (id: string) => apiFetch<Torneo>(`/api/v1/torneos/${id}`),
    crear: (data: CreateTorneoInput) =>
      apiFetch<Torneo>('/api/v1/torneos', { method: 'POST', body: JSON.stringify(data) }),
  },
};
