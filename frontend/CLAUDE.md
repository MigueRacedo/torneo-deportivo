# Frontend — React 19 + Vite

> Este CLAUDE.md aplica solo dentro de `/frontend`. Se complementa con el CLAUDE.md raíz.

## Stack
- **React 19** + **Vite** + **TypeScript**
- **TanStack Query** → todo el server state (cache, refetch, sync)
- **Zustand** → solo client state (auth, UI global)
- **TanStack Router** o React Router v7 → routing
- **React Hook Form + Zod** → formularios y validación
- **shadcn/ui + Tailwind CSS** → componentes de UI
- **@xyflow/react (React Flow)** → visualización del bracket ⭐
- **@microsoft/signalr** → actualización en vivo del bracket
- **Vitest** → testing

## Estructura del Proyecto
```
frontend/
├── src/
│   ├── routes/                 # Páginas (TanStack Router) o pages/
│   │   ├── login.tsx
│   │   ├── torneos/
│   │   │   ├── index.tsx              # Lista de torneos
│   │   │   ├── nuevo.tsx              # Crear torneo
│   │   │   └── $torneoId/
│   │   │       ├── index.tsx          # Detalle del torneo
│   │   │       ├── categorias.tsx
│   │   │       ├── competidores.tsx
│   │   │       └── llaves.tsx         # Bracket (React Flow)
│   │   └── reportes.tsx
│   ├── components/
│   │   ├── ui/                 # shadcn components (no modificar manualmente)
│   │   ├── torneos/
│   │   │   ├── TorneoCard.tsx
│   │   │   ├── TorneoForm.tsx
│   │   │   └── TorneoList.tsx
│   │   ├── bracket/            # ⭐ React Flow
│   │   │   ├── BracketView.tsx        # Componente principal con ReactFlow
│   │   │   ├── MatchNode.tsx          # Nodo custom para cada match
│   │   │   └── bracketLayout.ts       # Lógica de posicionamiento de nodos
│   │   ├── categorias/
│   │   ├── competidores/
│   │   └── layout/
│   │       ├── Navbar.tsx
│   │       ├── Sidebar.tsx
│   │       └── RoleGuard.tsx
│   ├── lib/
│   │   ├── api.ts             # Fetch wrapper tipado
│   │   ├── queryClient.ts     # Configuración de TanStack Query
│   │   ├── signalr.ts         # Conexión SignalR
│   │   └── utils.ts
│   ├── hooks/                 # Hooks de TanStack Query
│   │   ├── useTorneos.ts
│   │   ├── useCategorias.ts
│   │   ├── useCompetidores.ts
│   │   └── useBracket.ts
│   ├── types/
│   │   └── index.ts          # Tipos TypeScript del dominio
│   ├── store/
│   │   └── authStore.ts      # Zustand: solo auth
│   └── main.tsx
├── index.html
├── vite.config.ts
├── tailwind.config.ts
└── package.json
```

## Tipos TypeScript del Dominio
```typescript
// src/types/index.ts

export type TorneoEstado = 'Borrador' | 'Activo' | 'Finalizado';
export type TipoCompetencia = 'Combate' | 'Formas';
export type Sexo = 'M' | 'F';
export type RolUsuario = 'Coordinador' | 'Profesor' | 'Director';

export interface Torneo {
  id: string;
  nombre: string;
  fecha: string;
  lugar: string;
  imagenFlyer?: string;
  estado: TorneoEstado;
  totalCompetidores?: number;
}

export interface Categoria {
  id: string;
  torneoId: string;
  nombre: string;
  tipoCompetencia: TipoCompetencia;
  sexo: Sexo;
  rangoEdadMin: number;
  rangoEdadMax: number;
  rangoPesoMin: number;
  rangoPesoMax: number;
  rangoGraduacionMin: string;
  rangoGraduacionMax: string;
  totalCompetidores?: number;
}

export interface Competidor {
  id: string;
  torneoId: string;
  categoriaId: string;
  nombre: string;
  apellido: string;
  nombreCompleto: string;
  edad: number;
  graduacion: string;
  peso: number;
  altura: number;
  escuela: string;
  responsable: string;
  telefono?: string;
}

export interface Match {
  id: string;
  categoriaId: string;
  ronda: number;
  posicion: number;
  competidor1?: Competidor;
  competidor2?: Competidor;
  ganador?: Competidor;
  estado: 'Pendiente' | 'EnCurso' | 'Finalizado' | 'Bye';
}

export interface Bracket {
  categoriaId: string;
  categoriaNombre: string;
  totalRondas: number;
  rounds: Match[][];  // [ronda][matches en esa ronda]
}
```

## API Client (lib/api.ts)
```typescript
const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000';

class ApiError extends Error {
  constructor(public status: number, message: string) { super(message); }
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
    const error = await res.json().catch(() => ({ message: 'Error desconocido' }));
    throw new ApiError(res.status, error.message);
  }
  return res.status === 204 ? (undefined as T) : res.json();
}

export const api = {
  torneos: {
    listar: () => apiFetch<Torneo[]>('/api/v1/torneos'),
    obtener: (id: string) => apiFetch<Torneo>(`/api/v1/torneos/${id}`),
    crear: (data: CreateTorneoRequest) => apiFetch<Torneo>('/api/v1/torneos', { method: 'POST', body: JSON.stringify(data) }),
    editar: (id: string, data: UpdateTorneoRequest) => apiFetch<Torneo>(`/api/v1/torneos/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  },
  categorias: {
    listar: (torneoId: string) => apiFetch<Categoria[]>(`/api/v1/torneos/${torneoId}/categorias`),
    crear: (torneoId: string, data: CreateCategoriaRequest) => apiFetch<Categoria>(`/api/v1/torneos/${torneoId}/categorias`, { method: 'POST', body: JSON.stringify(data) }),
  },
  competidores: {
    listar: (torneoId: string) => apiFetch<Competidor[]>(`/api/v1/torneos/${torneoId}/competidores`),
    cargar: (torneoId: string, data: CreateCompetidorRequest) => apiFetch<Competidor>(`/api/v1/torneos/${torneoId}/competidores`, { method: 'POST', body: JSON.stringify(data) }),
  },
  llaves: {
    generar: (torneoId: string) => apiFetch<Bracket[]>(`/api/v1/torneos/${torneoId}/llaves/generar`, { method: 'POST' }),
    obtener: (torneoId: string, categoriaId: string) => apiFetch<Bracket>(`/api/v1/torneos/${torneoId}/llaves/${categoriaId}`),
    registrarGanador: (torneoId: string, matchId: string, ganadorId: string) =>
      apiFetch(`/api/v1/torneos/${torneoId}/llaves/${matchId}/ganador`, { method: 'PUT', body: JSON.stringify({ ganadorId }) }),
  },
};
```

## TanStack Query — Patrón de Hooks
```typescript
// hooks/useTorneos.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

// Query keys centralizadas (evita typos y facilita invalidación)
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
  });
}

export function useCrearTorneo() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: api.torneos.crear,
    onSuccess: () => qc.invalidateQueries({ queryKey: torneoKeys.all }),
  });
}
```

Uso en un componente:
```tsx
function TorneoList() {
  const { data: torneos, isLoading, error } = useTorneos();

  if (isLoading) return <Skeleton />;
  if (error) return <ErrorState message={error.message} />;

  return <div>{torneos.map(t => <TorneoCard key={t.id} torneo={t} />)}</div>;
}
```

## Bracket con React Flow (H0005 — Core Feature)
```tsx
// components/bracket/BracketView.tsx
// React Flow renderiza el bracket como un grafo de nodos conectados.
// Cada match es un nodo custom (MatchNode); las conexiones muestran
// qué match alimenta al siguiente.
//
// FLUJO:
// 1. useBracket(torneoId, categoriaId) trae el Bracket de la API
// 2. bracketLayout.ts convierte Bracket → { nodes, edges } de React Flow
//    - Cada match = un nodo con position {x, y}
//    - x = ronda * 280 (columnas)
//    - y = posición calculada con espaciado exponencial
//    - edges conectan match[ronda][i] → match[ronda+1][i/2]
// 3. <ReactFlow nodes={nodes} edges={edges} nodeTypes={{ match: MatchNode }} />
//
// Ver skills/react-flow-bracket.md para la implementación completa.
```

Detalles importantes del bracket:
- Layout de **izquierda a derecha**: Ronda 1 (izq) → Final (der)
- `nodeTypes={{ match: MatchNode }}` para nodos custom
- Los BYEs se muestran con estilo diferente ("Pasa directo")
- El ganador de cada match se resalta en verde
- Conectar SignalR para actualizar en vivo cuando se registra un ganador

## SignalR — Cliente
```typescript
// lib/signalr.ts
import * as signalR from '@microsoft/signalr';

export function crearConexionBracket(torneoId: string) {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_API_URL}/hubs/bracket`)
    .withAutomaticReconnect()
    .build();

  connection.start().then(() => connection.invoke('JoinTorneo', torneoId));
  return connection;
}

// En el componente del bracket, escuchar actualizaciones e invalidar la query:
// connection.on('MatchActualizado', () => qc.invalidateQueries({ queryKey: bracketKeys... }))
```

## Reglas del Frontend
1. **Todo server state** (datos de la API) vive en TanStack Query, nunca en useState/Zustand
2. **Zustand solo** para auth (token, usuario) y UI global (tema, sidebar abierto)
3. **Siempre** definir query keys centralizadas (objeto `{entidad}Keys`)
4. Las mutaciones **invalidan** las queries relevantes en `onSuccess`
5. Los formularios usan **React Hook Form + Zod**
6. La validación del cliente es UX, no seguridad — el backend siempre re-valida
7. El `RoleGuard` protege todas las rutas del dashboard
8. El bracket (React Flow) debe ser responsive (pan + zoom en mobile)

## UX/UI y Accesibilidad (WCAG 2.2 AA) — obligatorio
> Guía completa con ejemplos en Tailwind: `skills/ux-ui-guidelines.md`. Leerla antes
> de crear o tocar cualquier componente visual.

Resumen de las reglas no negociables:
- **Color (regla 60-30-10):** blanco = base/fondo (60%), azul = navegación/secundario (30%),
  rojo = SOLO CTAs y acciones principales (10%, ej. "Declarar ganador"). Nunca comunicar
  un estado solo con color (siempre + texto/ícono). Contraste texto/fondo ≥ 4.5:1.
- **Tipografía:** Sans Serif, mínimo 12px, interlineado ≥ 1.5, alineado a la izquierda
  (nunca `text-justify`). Jerarquía de headings sin saltos (`h1`→`h2`→`h3`).
- **Layout:** spacing en múltiplos de 8pt (`p-2`, `p-4`, `p-6`, `p-8`...). Grid de 12
  columnas en desktop, 6-8 en tablet, 1-4 en mobile.
- **Interacción (Ley de Fitts):** áreas clickeables ≥ 44x44px (mínimo absoluto 24x24px).
  Foco de teclado siempre visible — **prohibido** `outline: none`/`focus:outline-none`
  sin reemplazo (usar `focus-visible:ring-2` o `focus-visible:outline`).
- **Prevención de errores:** acciones irreversibles (avanzar ronda, declarar ganador,
  eliminar) necesitan Undo o confirmación. Todo `<input>` lleva `<Label>` visible
  (el placeholder no reemplaza al label). Mensajes de error específicos y accionables,
  nunca "Se produjo un error".
- **HTML semántico primero:** `<header>`, `<nav>`, `<main>`, `<aside>` antes de pensar
  en ARIA o CSS complejo.

Esto se revisa automáticamente con `/code-review frontend` (ver checklist de
Accesibilidad/UX en `.claude/commands/code-review.md`).

## Variables de Entorno (.env)
```
VITE_API_URL=http://localhost:5000
VITE_APP_NAME=TorneoApp
```
> Nota: en Vite las env vars del cliente empiezan con `VITE_` (no `NEXT_PUBLIC_`)
