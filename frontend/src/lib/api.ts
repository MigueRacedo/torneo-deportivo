import { useAuthStore } from '@/store/authStore';
import type {
  Categoria,
  Competidor,
  CreateCategoriaInput,
  CreateCompetidorInput,
  CreateTorneoInput,
  LoginInput,
  LoginResult,
  Torneo,
  UpdateCompetidorInput,
  UpdateTorneoInput,
} from '@/types';

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000';

export class ApiError extends Error {
  status: number;

  constructor(status: number, message: string) {
    super(message);
    this.status = status;
  }
}

// Extrae el primer mensaje de error útil de la respuesta del backend.
// FastEndpoints devuelve `errors` como objeto ({ campo: ["msg"] }) y nuestro
// middleware como arreglo ([{ field, message }]); soportamos ambas formas.
function extraerMensajeError(body: unknown): string | null {
  if (!body || typeof body !== 'object') return null;
  const { errors, message } = body as { errors?: unknown; message?: unknown };

  if (Array.isArray(errors) && errors.length > 0) {
    const first = errors[0];
    if (typeof first === 'string') return first;
    if (first && typeof first === 'object' && 'message' in first) {
      return String((first as { message: unknown }).message);
    }
  } else if (errors && typeof errors === 'object') {
    const first = Object.values(errors as Record<string, unknown>)[0];
    if (Array.isArray(first) && first.length > 0) return String(first[0]);
    if (typeof first === 'string') return first;
  }

  return typeof message === 'string' ? message : null;
}

async function apiFetch<T>(path: string, options?: RequestInit): Promise<T> {
  const token = useAuthStore.getState().token;
  // Solo enviamos Content-Type cuando hay cuerpo: en un GET sin body, el header
  // haría que FastEndpoints intente deserializar un JSON vacío y devuelva 400.
  const hasBody = options?.body != null;
  const res = await fetch(`${BASE_URL}${path}`, {
    ...options,
    headers: {
      ...(hasBody ? { 'Content-Type': 'application/json' } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options?.headers,
    },
  });

  if (!res.ok) {
    if (res.status === 401) {
      useAuthStore.getState().logout();
    }

    const body = await res.json().catch(() => null);
    const message = extraerMensajeError(body) ?? 'No se pudo completar la operación. Intentá de nuevo.';
    throw new ApiError(res.status, message);
  }

  return res.status === 204 ? (undefined as T) : res.json();
}

export const api = {
  auth: {
    login: (data: LoginInput) =>
      apiFetch<LoginResult>('/api/v1/auth/login', { method: 'POST', body: JSON.stringify(data) }),
  },
  torneos: {
    listar: () => apiFetch<Torneo[]>('/api/v1/torneos'),
    obtener: (id: string) => apiFetch<Torneo>(`/api/v1/torneos/${id}`),
    crear: (data: CreateTorneoInput) =>
      apiFetch<Torneo>('/api/v1/torneos', { method: 'POST', body: JSON.stringify(data) }),
    editar: (id: string, data: UpdateTorneoInput) =>
      apiFetch<Torneo>(`/api/v1/torneos/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  },
  categorias: {
    listar: (torneoId: string) =>
      apiFetch<Categoria[]>(`/api/v1/torneos/${torneoId}/categorias`),
    crear: (torneoId: string, data: CreateCategoriaInput) =>
      apiFetch<Categoria>(`/api/v1/torneos/${torneoId}/categorias`, {
        method: 'POST',
        body: JSON.stringify(data),
      }),
    editar: (torneoId: string, id: string, data: CreateCategoriaInput) =>
      apiFetch<Categoria>(`/api/v1/torneos/${torneoId}/categorias/${id}`, {
        method: 'PUT',
        body: JSON.stringify(data),
      }),
  },
  competidores: {
    listar: (torneoId: string) =>
      apiFetch<Competidor[]>(`/api/v1/torneos/${torneoId}/competidores`),
    cargar: (torneoId: string, data: CreateCompetidorInput) =>
      apiFetch<Competidor>(`/api/v1/torneos/${torneoId}/competidores`, {
        method: 'POST',
        body: JSON.stringify(data),
      }),
    editar: (torneoId: string, id: string, data: UpdateCompetidorInput) =>
      apiFetch<Competidor>(`/api/v1/torneos/${torneoId}/competidores/${id}`, {
        method: 'PUT',
        body: JSON.stringify(data),
      }),
    eliminar: (torneoId: string, id: string) =>
      apiFetch<void>(`/api/v1/torneos/${torneoId}/competidores/${id}`, { method: 'DELETE' }),
  },
};
